import subprocess

# 先运行 test.py 抓取 cookies
subprocess.run(["python", "test.py"], check=True)

# 再运行 getSCHL.py 抓取数据
subprocess.run(["python", "getSCHL.py"], check=True)
