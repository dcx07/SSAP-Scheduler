# -*- coding: utf-8 -*-
import sys
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.edge.service import Service
from selenium.webdriver.edge.options import Options
import time
import json

# �������в�����ȡ�û���������
username = sys.argv[1]
password = sys.argv[2]

# ���� Edge �����Ϊ��ͷģʽ
options = Options()
options.add_argument("--headless")  # ������ͷģʽ
options.add_argument("--disable-gpu")  # ���� GPU ���٣���ѡ�����������ԣ�

# ���� Edge
service = Service("msedgedriver.exe")
driver = webdriver.Edge(service=service, options=options)

# �򿪵�¼ҳ��
driver.get("https://sendeltastudent.schoolis.cn")
time.sleep(2)

# ��λ�û������������
driver.find_element(By.XPATH, "//input[@ng-model='$ctrl.loginName']").send_keys(username)

# ��λ�������������
driver.find_element(By.XPATH, "//input[@ng-model='$ctrl.passWord']").send_keys(password)

# �����¼��ť
driver.find_element(By.XPATH, "//button[@ng-click='$ctrl.login()']").click()

time.sleep(5)

# ��ȡ��¼��� cookies
cookies = driver.get_cookies()


# ���� cookies
with open("cookies.json", "w", encoding="utf-8") as f:
    json.dump(cookies, f, ensure_ascii=False, indent=2)

driver.quit()
