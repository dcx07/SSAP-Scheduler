import subprocess
import json

def main():
    # 从 config.json 读取用户名和密码
    with open("config.json", "r", encoding="utf-8") as f:
        config = json.load(f)
    username = config["username"]
    password = config["password"]

    # 运行 GetCK.py 抓取 cookies
    subprocess.run(["python", "GetCK_AUTO.py", username, password], check=True)

    # 运行 getSCHL.py 抓取数据
    subprocess.run(["python", "getSCHL.py"], check=True)

if __name__ == "__main__":
    main()

