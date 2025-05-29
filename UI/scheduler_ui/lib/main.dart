import 'package:flutter/material.dart';
import 'dart:convert';
import 'dart:io';
import 'package:intl/intl.dart';
import 'package:intl/date_symbol_data_local.dart';

// 登录页
class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _usernameController = TextEditingController();
  final _passwordController = TextEditingController();
  bool _isLoading = false;

  Future<void> _login() async {
    setState(() => _isLoading = true);
    try {
      // 动态获取 Backend 路径
      final backendDir = await getBackendDir();
      final configPath = '$backendDir/config.json';
      final configMap = {
        'username': _usernameController.text,
        'password': _passwordController.text,
      };
      await File(configPath).writeAsString(jsonEncode(configMap));

      // 跳转到日程页
      if (mounted) {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(
            builder: (_) => SchedulePage(
              username: _usernameController.text,
              password: _passwordController.text,
            ),
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text('登录失败: $e')));
      }
    } finally {
      setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFFDF5E6), // 黄白色背景
      appBar: PreferredSize(
        preferredSize: const Size.fromHeight(90),
        child: AppBar(
          backgroundColor: Colors.transparent,
          elevation: 0,
          flexibleSpace: Container(
            height: 90,
            decoration: const BoxDecoration(
              color: Color(0xFF6C63FF), // 蓝紫色长条
              borderRadius: BorderRadius.only(
                bottomLeft: Radius.circular(24),
                bottomRight: Radius.circular(24),
              ),
            ),
            alignment: Alignment.bottomLeft,
            padding: const EdgeInsets.only(left: 32, bottom: 18),
            child: const Text(
              '登录',
              style: TextStyle(
                color: Colors.white,
                fontSize: 28,
                fontWeight: FontWeight.w600,
                letterSpacing: 2,
                fontFamily: 'NotoSansSC',
              ),
            ),
          ),
        ),
      ),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            TextField(
              controller: _usernameController,
              decoration: const InputDecoration(
                labelText: '用户名',
                labelStyle: TextStyle(
                  color: Color(0xFFFFB870), // 低饱和度橙色
                  fontSize: 18,
                  fontWeight: FontWeight.w600,
                  fontFamily: 'NotoSansSC',
                ),
                enabledBorder: UnderlineInputBorder(
                  borderSide: BorderSide(color: Color(0xFFFFB870)),
                ),
                focusedBorder: UnderlineInputBorder(
                  borderSide: BorderSide(color: Color(0xFFFFB870), width: 2),
                ),
              ),
              style: const TextStyle(
                fontSize: 18,
                fontFamily: 'NotoSansSC',
                fontWeight: FontWeight.w600,
              ),
            ),
            const SizedBox(height: 16),
            TextField(
              controller: _passwordController,
              decoration: const InputDecoration(
                labelText: '密码',
                labelStyle: TextStyle(
                  color: Color(0xFFFFB870),
                  fontSize: 18,
                  fontWeight: FontWeight.w600,
                  fontFamily: 'NotoSansSC',
                ),
                enabledBorder: UnderlineInputBorder(
                  borderSide: BorderSide(color: Color(0xFFFFB870)),
                ),
                focusedBorder: UnderlineInputBorder(
                  borderSide: BorderSide(color: Color(0xFFFFB870), width: 2),
                ),
              ),
              obscureText: true,
              style: const TextStyle(
                fontSize: 18,
                fontFamily: 'NotoSansSC',
                fontWeight: FontWeight.w600,
              ),
            ),
            const SizedBox(height: 32),
            _isLoading
                ? const CircularProgressIndicator()
                : SizedBox(
                    width: double.infinity,
                    height: 48,
                    child: ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        backgroundColor:
                            const Color.fromARGB(255, 133, 128, 240),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                        elevation: 2,
                      ),
                      onPressed: _login,
                      child: const Text(
                        '登录',
                        style: TextStyle(
                          fontSize: 20,
                          fontWeight: FontWeight.w600,
                          letterSpacing: 2,
                          color: Colors.white,
                          fontFamily: 'NotoSansSC',
                        ),
                      ),
                    ),
                  ),
          ],
        ),
      ),
    );
  }
}

// 日程页
class SchedulePage extends StatefulWidget {
  final String username;
  final String password;

  const SchedulePage({
    super.key,
    required this.username,
    required this.password,
  });

  @override
  SchedulePageState createState() => SchedulePageState();
}

class SchedulePageState extends State<SchedulePage> {
  List<dynamic> _courses = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _saveConfigAndFetchSchedule();
  }

  Future<void> _saveConfigAndFetchSchedule() async {
    setState(() => _isLoading = true);
    try {
      final backendDir = await getBackendDir();
      final configPath = '$backendDir/config.json';
      final schedulePath = '$backendDir/schedule_grouped.json';

      // 保存配置
      final configMap = {
        'username': widget.username,
        'password': widget.password,
      };

      await File(configPath).writeAsString(jsonEncode(configMap));
      final pythonScript = 'main.py';
      final process = await Process.run(
          'python',
          [
            pythonScript,
          ],
          workingDirectory: backendDir);

      if (process.exitCode != 0) {
        throw Exception('Python script error: ${process.stderr}');
      }

      // 读取生成的日程数据
      if (await File(schedulePath).exists()) {
        final jsonString = await File(schedulePath).readAsString();
        final data = jsonDecode(jsonString);

        // 获取今天是周几
        final weekDays = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
        final todayKey = weekDays[DateTime.now().weekday - 1];

        // 只显示今天的课程
        List todayCourses = [];
        if (data[todayKey] != null && data[todayKey]['courses'] is List) {
          todayCourses = data[todayKey]['courses'];
        }

        setState(() {
          _courses = todayCourses.reversed.toList();
          _isLoading = false;
        });
      } else {
        throw Exception('Schedule file not found');
      }
    } catch (e) {
      debugPrint('Error processing schedule: $e');
      setState(() => _isLoading = false);
      if (mounted) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text('更新日程失败: ${e.toString()}')));
      }
    }
  }

  Widget _buildCourseTile(Map<String, dynamic> course) {
    return Container(
      margin: const EdgeInsets.symmetric(vertical: 8, horizontal: 16),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.95),
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          BoxShadow(
            color: Colors.blueGrey.withValues(alpha: 0.1),
            blurRadius: 8,
            spreadRadius: 2,
          ),
        ],
      ),
      child: ListTile(
        leading: Text(
          course['emoji'] ?? '',
          style: const TextStyle(
            fontSize: 24,
            fontFamily: 'NotoSansSC',
            fontWeight: FontWeight.w600,
          ),
        ),
        title: Text(
          course['name'] ?? '',
          style: const TextStyle(
            fontWeight: FontWeight.w600,
            color: Color(0xFF4A5568),
            fontFamily: 'NotoSansSC',
          ),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              '⏰ ${course['start'] ?? ''} - ${course['end'] ?? ''}',
              style: const TextStyle(
                color: Color(0xFF718096),
                fontFamily: 'NotoSansSC',
                fontWeight: FontWeight.w600,
              ),
            ),
            Text(
              '📍 ${course['room'] ?? ''}',
              style: const TextStyle(
                color: Color(0xFF718096),
                fontFamily: 'NotoSansSC',
                fontWeight: FontWeight.w600,
              ),
            ),
            Text(
              '👨‍🏫 ${course['teacher'] ?? ''}',
              style: const TextStyle(
                color: Color(0xFF718096),
                fontFamily: 'NotoSansSC',
                fontWeight: FontWeight.w600,
              ),
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFFDF5E6),
      appBar: AppBar(
        title: Row(
          children: [
            Icon(Icons.calendar_today, color: Colors.blue[900]),
            const SizedBox(width: 10),
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  widget.username,
                  style: const TextStyle(
                    fontSize: 16,
                    fontFamily: 'NotoSansSC',
                    fontWeight: FontWeight.w600,
                  ),
                ),
                Text(
                  DateFormat('yyyy年M月d日 EEEE', 'zh_CN').format(DateTime.now()),
                  style: const TextStyle(
                    fontSize: 12,
                    fontFamily: 'NotoSansSC',
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
          ],
        ),
        backgroundColor: Colors.white,
        elevation: 0,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh, color: Color(0xFF6C63FF)),
            onPressed: _saveConfigAndFetchSchedule,
          ),
        ],
      ),
      body: _isLoading
          ? const Center(
              child: CircularProgressIndicator(color: Color(0xFF6C63FF)),
            )
          : ListView.builder(
              itemCount: _courses.length,
              itemBuilder: (context, index) {
                return _buildCourseTile(
                  Map<String, dynamic>.from(_courses[index]),
                );
              },
            ),
    );
  }
}

Future<String> getBackendDir() async {
  // 获取UI目录的父目录的父目录，然后拼接Backend
  final uiDir = Directory.current;
  final projectRoot = uiDir.parent.parent; // D:\SSAP-Scheduler
  final backendDir = Directory('${projectRoot.path}/Backend');
  if (!await backendDir.exists()) {
    await backendDir.create(recursive: true);
  }
  return backendDir.absolute.path;
}

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await initializeDateFormatting('zh_CN', null);
  runApp(
    MaterialApp(
      home: const LoginPage(),
      theme: ThemeData(
        fontFamily: 'NotoSansSC', // 全局通用中文字体
      ),
    ),
  );
}
