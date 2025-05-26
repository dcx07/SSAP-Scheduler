# -*- coding: utf-8 -*-
import subprocess
import json

def main():
    # 从 config.json 读取用户名和密码
    with open("config.json", "r", encoding="utf-8") as f:
        config = json.load(f)
    username = config["username"]
    password = config["password"]

    # 运行 GetCK.py 抓取 cookies
    subprocess.run(["getCK_AUTO.exe", username, password], check=True)

    # 运行 getSCHL.py 抓取数据
    subprocess.run(["getSCHL.exe"], check=True)

if __name__ == "__main__":
    main()

