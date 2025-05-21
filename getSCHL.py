# -*- coding: utf-8 -*-
import requests
import urllib3
import json
import re
from datetime import datetime, timedelta
from collections import defaultdict

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

# 1. 加载 cookies
cookie_keys = [
    "SessionId",
    "HMACCOUNT",
    "Hm_lvt_4f71d5631552c192190713bef03fe1d8",
    "Hm_lpvt_4f71d5631552c192190713bef03fe1d8"
]

with open("cookies.json", "r", encoding="utf-8") as f:
    cookies_list = json.load(f)
cookies = {c['name']: c['value'] for c in cookies_list if c['name'] in cookie_keys}

# === 新增：自动计算本周周日和周六 ===
def get_week_range():
    today = datetime.today()
    # 以周日为一周的第一天
    # weekday(): 周一=0, ..., 周日=6
    days_since_sunday = (today.weekday() + 1) % 7
    sunday = today - timedelta(days=days_since_sunday)
    saturday = sunday + timedelta(days=6)
    return sunday.strftime('%Y-%m-%d'), saturday.strftime('%Y-%m-%d')

beginTime, endTime = get_week_range()
# ================================

# 2. 请求课表接口
url = "https://sendeltastudent.schoolis.cn/api/Schedule/ListScheduleByParent"
payload = {
    "beginTime": beginTime,
    "endTime": endTime
}
headers = {
    "Content-Type": "application/json",
    "Referer": "https://sendeltastudent.schoolis.cn/",
    "User-Agent": "Mozilla/5.0"
}
response = requests.post(url, json=payload, cookies=cookies, headers=headers, verify=False)
data = response.json()

# 3. 微软时间格式解析函数
def parse_ms_date(ms_date):
    match = re.match(r'/Date\((\d+)([+-]\d{4})\)/', ms_date)
    if not match:
        return ms_date
    timestamp_ms = int(match.group(1))
    offset_str = match.group(2)
    dt = datetime.utcfromtimestamp(timestamp_ms / 1000)
    sign = 1 if offset_str.startswith('+') else -1
    hours = int(offset_str[1:3])
    minutes = int(offset_str[3:])
    offset = timedelta(hours=hours, minutes=minutes)
    dt = dt + sign * offset
    return dt

# 4. 分组保存结构化数据
grouped = defaultdict(list)

for item in data["data"]:
    name = item["name"]
    room = item.get("playgroundName") or "未指定教室"
    start = parse_ms_date(item["beginTime"])
    end = parse_ms_date(item["endTime"])
    weekday = start.strftime("%a")
    teachers = item.get("teacherList", [])
    teacher_names = ", ".join(t["name"] for t in teachers) if teachers else "未知教师"

    grouped[weekday].append({
        "name": name,
        "room": room,
        "start": start.isoformat(),
        "end": end.isoformat(),
        "teacher": teacher_names
    })

# 5. 保存为 JSON 文件
with open("schedule_grouped.json", "w", encoding="utf-8") as f:
    json.dump(grouped, f, ensure_ascii=False, indent=2)

print("课表已成功保存为 schedule_grouped.json")
