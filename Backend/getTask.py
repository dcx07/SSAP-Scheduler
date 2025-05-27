# -*- coding: utf-8 -*-
import requests
import json
import re
from datetime import datetime, timedelta

# 1. 加载 cookies
cookie_keys = ["SessionId", "HMACCOUNT", "Hm_lvt_4f71d5631552c192190713bef03fe1d8", "Hm_lpvt_4f71d5631552c192190713bef03fe1d8"]
with open("cookies.json", "r", encoding="utf-8") as f:
    cookies_list = json.load(f)
cookies = {c['name']: c['value'] for c in cookies_list if c['name'] in cookie_keys}

headers = {
    "Referer": "https://sendeltastudent.schoolis.cn/",
    "User-Agent": "Mozilla/5.0"
}

# 2. 获取当前学期
def parse_dotnet_date(dotnet_str):
    match = re.match(r"/Date\((\d+)([+-]\d{4})\)/", dotnet_str)
    if not match:
        return None
    timestamp_ms = int(match.group(1))
    offset_str = match.group(2)
    dt_utc = datetime.utcfromtimestamp(timestamp_ms / 1000)
    offset_hours = int(offset_str[:3])
    offset_minutes = int(offset_str[0] + offset_str[3:])
    offset = timedelta(hours=offset_hours, minutes=offset_minutes)
    return dt_utc + offset

term_url = "https://sendeltastudent.schoolis.cn/api/School/GetSchoolSemesters"
resp = requests.get(term_url, cookies=cookies, headers=headers, verify=False)
term_list = resp.json()["data"]
current_term = next((t for t in term_list if t["isNow"]), None)

if not current_term:
    print("❌ 未找到当前学期")
    exit()

semester_id = current_term["id"]
start_date = parse_dotnet_date(current_term["startDate"]).strftime("%Y-%m-%d")
end_date = parse_dotnet_date(current_term["endDate"]).strftime("%Y-%m-%d")

print(f"🎓 当前学期：{current_term['name']} (ID: {semester_id})")
print(f"📅 时间范围：{start_date} ~ {end_date}")

# 3. 抓取任务（分页）
unfinished_tasks = []
page = 1
page_size = 20

while True:
    task_url = (
        "https://sendeltastudent.schoolis.cn/api/LearningTask/GetList"
        f"?semesterId={semester_id}"
        f"&subjectId=null&typeId=null&key=&beginTime={start_date}&endTime={end_date}"
        f"&mode=null&pageIndex={page}&pageSize={page_size}"
    )
    task_resp = requests.get(task_url, cookies=cookies, headers=headers, verify=False)
    data = task_resp.json().get("data", {})
    task_list = data.get("list", [])

    if not task_list:
        break  # 没有更多任务了

    for task in task_list:
        if task.get("learningTaskState") != 1:  # 1 表示已完成
            unfinished_tasks.append({
                "id": task["id"],
                "name": task["name"],
                "subject": task["subjectName"],
                "type": task["typeName"],
                "begin": parse_dotnet_date(task["beginTime"]).strftime("%Y-%m-%d %H:%M"),
                "end": parse_dotnet_date(task["endTime"]).strftime("%Y-%m-%d %H:%M"),
                "state": "未完成",
                "score": task.get("score", None)
            })

    # 判断是否还有下一页
    if len(task_list) < page_size:
        break
    page += 1

# 4. 输出未完成任务
print(f"\n📌 共找到 {len(unfinished_tasks)} 个未完成任务：\n")
for task in unfinished_tasks:
    print(f"{task['name']} | {task['subject']} | {task['type']} | 截止：{task['end']} | 状态：{task['state']}")
