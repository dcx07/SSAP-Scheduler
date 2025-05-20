# -*- coding: utf-8 -*-
import requests
import urllib3
import json
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

cookie_keys = [
    "SessionId",
    "HMACCOUNT",
    "Hm_lvt_4f71d5631552c192190713bef03fe1d8",
    "Hm_lpvt_4f71d5631552c192190713bef03fe1d8"
]

with open("cookies.json", "r", encoding="utf-8") as f:
    cookies_list = json.load(f)
cookies = {c['name']: c['value'] for c in cookies_list if c['name'] in cookie_keys}


url = "https://sendeltastudent.schoolis.cn/api/Schedule/ListScheduleByParent"

# ���������ѯ����ֹ���ڣ�����ÿ�ܲ�ѯһ�Σ�
payload = {
    "beginTime": "2025-05-18",
    "endTime": "2025-05-24"
}

headers = {
    "Content-Type": "application/json",
    "Referer": "https://sendeltastudent.schoolis.cn/",  # ��Щ��������Ҫ Referer ������
    "User-Agent": "Mozilla/5.0"
}

response = requests.post(url, json=payload, cookies=cookies, headers=headers, verify=False)

data = response.json()
for item in data["data"]:
    print(f"{item['name']} at {item['playgroundName']} from {item['beginTime']} to {item['endTime']}")
