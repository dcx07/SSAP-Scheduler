# -*- coding: utf-8 -*-
import requests
import json
import re
from datetime import datetime, timedelta

import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

# ------------------ 读取 cookies ------------------
with open("cookies.json", "r", encoding="utf-8") as f:
    cookie_list = json.load(f)
cookie_keys = [
    "SessionId",
    "HMACCOUNT",
    "Hm_lvt_4f71d5631552c192190713bef03fe1d8",
    "Hm_lpvt_4f71d5631552c192190713bef03fe1d8"
]
cookies = {c["name"]: c["value"] for c in cookie_list if c["name"] in cookie_keys}

# ------------------ 时间解析 ------------------
def parse_ms_date(ms_date):
    match = re.match(r"/Date\((\d+)([+-]\d{4})\)/", ms_date)
    if not match:
        return None
    timestamp_ms = int(match.group(1))
    offset_str = match.group(2)
    dt = datetime.utcfromtimestamp(timestamp_ms / 1000)
    sign = 1 if offset_str.startswith('+') else -1
    offset = timedelta(hours=int(offset_str[1:3]), minutes=int(offset_str[3:]))
    dt += sign * offset
    return dt

# ------------------ 获取所有学期 ------------------
def get_all_semesters():
    url = "https://sendeltastudent.schoolis.cn/api/School/GetSchoolSemesters"
    res = requests.get(url, cookies=cookies, verify=False)
    semesters = res.json().get("data", [])
    result = []
    for sem in semesters:
        sem_id = sem["id"]
        start = parse_ms_date(sem["startDate"])
        end = parse_ms_date(sem["endDate"])
        if start and end:
            result.append({
                "id": sem_id,
                "start": start.date().isoformat(),
                "end": end.date().isoformat()
            })
    return result

# ------------------ 判断当前学期 ------------------
def get_current_semester(semesters):
    today = datetime.now().date()
    for sem in semesters:
        start_date = datetime.strptime(sem["start"], "%Y-%m-%d").date()
        end_date = datetime.strptime(sem["end"], "%Y-%m-%d").date()
        if start_date <= today <= end_date:
            return sem
    return None

# ------------------ 抓取一个学期的任务 ------------------
def get_tasks_for_semester(semester_id, start, end):
    all_tasks = []
    page = 1
    while True:
        url = (
            f"https://sendeltastudent.schoolis.cn/api/LearningTask/GetList"
            f"?semesterId={semester_id}&subjectId=null&typeId=null&key=&"
            f"beginTime={start}&endTime={end}&mode=null&pageIndex={page}&pageSize=50"
        )
        res = requests.get(url, cookies=cookies, verify=False)
        data = res.json().get("data", {}).get("list", [])
        if not data:
            break
        all_tasks.extend(data)
        page += 1
    return all_tasks

# ------------------ 整理为按日归类 ------------------
def group_tasks_by_day(tasks):
    grouped = {}
    for task in tasks:
        if task.get("finishState") == 1:
            continue  
        due = parse_ms_date(task.get("endTime")) or datetime.now()
        date_str = due.strftime("%Y-%m-%d")
        item = {
            "title": task.get("name", "未命名任务"),
            "course": task.get("subjectName", "未知课程"),
            "type": task.get("typeName", ""),
            "due": due.strftime("%Y-%m-%dT%H:%M"),
            "teacher": task.get("lastEditorName", "")
        }
        grouped.setdefault(date_str, []).append(item)
    return grouped

# ------------------ 主流程 ------------------
# 获取所有学期
semesters = get_all_semesters()

# 获取当前学期
current_semester = get_current_semester(semesters)
if not current_semester:
    print("⚠️ 当前日期不在任何学期范围内")
    exit(1)

# 获取当前学期的任务
all_tasks = get_tasks_for_semester(
    current_semester["id"], 
    current_semester["start"], 
    current_semester["end"]
)

grouped = group_tasks_by_day(all_tasks)

with open("tasks.json", "w", encoding="utf-8") as f:
    json.dump(grouped, f, ensure_ascii=False, indent=2)

print(f"✅ {current_semester['start']} 至 {current_semester['end']} 的学习任务已保存为 tasks.json")
