# -*- coding: utf-8 -*-
import subprocess
import os
import sys
import json

def run_executable(exe_name, py_fallback=None):
    """运行可执行文件，如果失败则尝试Python脚本"""
    # Get current directory to construct absolute paths
    current_dir = os.path.dirname(os.path.abspath(__file__))
    
    # Prevent recursion by never running main.exe or main.py from this script
    if exe_name in ["main.exe", "main"] or (py_fallback and py_fallback in ["main.py", __file__]):
        print(f"跳过 {exe_name} 以防止递归调用")
        return False
    
    # Try to find the executable in common locations
    exe_paths = [
        exe_name,  # Current directory
        os.path.join(current_dir, exe_name),  # Same directory as script
        os.path.join(current_dir, "dist", exe_name),  # dist subdirectory
    ]
    
    for exe_path in exe_paths:
        if os.path.exists(exe_path):
            try:
                print(f"运行可执行文件: {exe_path}")
                subprocess.run([exe_path], check=True)
                return True
            except (subprocess.CalledProcessError, PermissionError, OSError) as e:
                print(f"可执行文件 {exe_path} 执行失败: {e}")
                continue
    
    # If no executable found or all failed, try Python fallback
    print(f"未找到可用的可执行文件 {exe_name}")
    if py_fallback and os.path.exists(py_fallback):
        print(f"尝试运行 Python 脚本: {py_fallback}")
        try:
            subprocess.run([sys.executable, py_fallback], check=True)
            return True
        except subprocess.CalledProcessError as py_e:
            print(f"Python 脚本也执行失败: {py_e}")
            return False
    else:
        print(f"未找到备用 Python 脚本: {py_fallback}")
        return False

def get_cookies():
    """获取 cookies，优先使用 exe，失败时使用 Python 脚本"""
    return run_executable("getCK_new.exe", "getCK_new.py")

def get_schedule():
    """获取课表数据，优先使用 exe，失败时使用 Python 脚本"""
    return run_executable("getSCHL.exe", "getSCHL.py")

def main():
    print("开始获取 cookies...")
    if not get_cookies():
        print("❌ 获取 cookies 失败，程序终止")
        return False
    
    print("开始获取课表数据...")
    if not get_schedule():
        print("❌ 获取课表数据失败，程序终止")
        return False
    
    print("✅ 所有任务完成成功")
    return True

if __name__ == "__main__":
    main()
