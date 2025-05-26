# -*- coding: utf-8 -*-
import requests
import json
import re
from datetime import datetime

# 1. 加载 cookies（你自己的逻辑）
cookie_keys = ["SessionId", "HMACCOUNT", "Hm_lvt_4f71d5631552c192190713bef03fe1d8", "Hm_lpvt_4f71d5631552c192190713bef03fe1d8"]
with open("cookies.json", "r", encoding="utf-8") as f:
    cookies_list = json.load(f)
cookies = {c['name']: c['value'] for c in cookies_list if c['name'] in cookie_keys}

# 2. 请求学期列表接口（正确 URL）
url = "https://sendeltastudent.schoolis.cn/api/School/GetSchoolSemesters"
headers = {
    "Content-Type": "application/json",
    "Referer": "https://sendeltastudent.schoolis.cn/",
    "User-Agent": "Mozilla/5.0"
}
response = requests.get(url, json={}, cookies=cookies, headers=headers, verify=False)
term_list = response.json()["data"]

# 3. 提取当前学期
def extract_timestamp(date_str):
    match = re.search(r"/Date\((\d+)", date_str)
    if match:
        timestamp = int(match.group(1)) / 1000
        return datetime.fromtimestamp(timestamp)
    return None

current_term = next((term for term in term_list if term["isNow"]), None)
if current_term:
    start_date = extract_timestamp(current_term["startDate"])
    end_date = extract_timestamp(current_term["endDate"])
    print("当前学期：", current_term["name"])
    print("开始日期：", start_date.strftime("%Y-%m-%d"))
    print("结束日期：", end_date.strftime("%Y-%m-%d"))
else:
    print("未找到当前学期")
