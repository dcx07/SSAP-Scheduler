# -*- coding: utf-8 -*-
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.edge.service import Service
import time
import json

# ���� Edge
service = Service("msedgedriver.exe")
driver = webdriver.Edge(service=service)

# �򿪵�¼ҳ��
driver.get("https://sendeltastudent.schoolis.cn")
time.sleep(2)

# ��λ�û������������
driver.find_element(By.XPATH, "//input[@ng-model='$ctrl.loginName']").send_keys("dengchengxuan")

# ��λ�������������
driver.find_element(By.XPATH, "//input[@ng-model='$ctrl.passWord']").send_keys("dyjwo3-majjyg-Porris")

# �����¼��ť
driver.find_element(By.XPATH, "//button[@ng-click='$ctrl.login()']").click()

time.sleep(5)

# ��ȡ��¼��� cookies
cookies = driver.get_cookies()
print("Cookies:", cookies)

# ���� cookies
with open("cookies.json", "w", encoding="utf-8") as f:
    json.dump(cookies, f, ensure_ascii=False, indent=2)

driver.quit()
