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
            builder:
                (_) => SchedulePage(
                  username: _usernameController.text,
                  password: _passwordController.text,
                ),
          ),
        );
      }
    } catch (e) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text('登录失败: $e')));
    } finally {
      setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('登录')),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            TextField(
              controller: _usernameController,
              decoration: const InputDecoration(labelText: '用户名'),
            ),
            const SizedBox(height: 16),
            TextField(
              controller: _passwordController,
              decoration: const InputDecoration(labelText: '密码'),
              obscureText: true,
            ),
            const SizedBox(height: 32),
            _isLoading
                ? const CircularProgressIndicator()
                : ElevatedButton(onPressed: _login, child: const Text('登录')),
          ],
        ),
      ),
    );
  }
}

// 日程页
class SchedulePage extends StatefulWidget {
  final String username;
  final String password; // 添加密码参数

  const SchedulePage({
    super.key,
    required this.username,
    required this.password,
  });

  @override
  _SchedulePageState createState() => _SchedulePageState();
}

class _SchedulePageState extends State<SchedulePage> {
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
      final process = await Process.run('python', [
        pythonScript,
      ], workingDirectory: backendDir);

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
      print('Error processing schedule: $e');
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
      margin: EdgeInsets.symmetric(vertical: 8, horizontal: 16),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.95),
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          // BoxShadow 是 Flutter 提供的类，直接使用即可，原代码无问题，此处直接保留
          BoxShadow(
            color: Colors.blueGrey.withValues(alpha: 0.1),
            blurRadius: 8,
            spreadRadius: 2,
          ),
        ],
      ),
      child: ListTile(
        leading: Text(course['emoji'] ?? '', style: TextStyle(fontSize: 24)),
        title: Text(
          course['name'] ?? '',
          style: TextStyle(
            fontFamily: 'HarmonyOS Sans',
            fontWeight: FontWeight.w600,
            color: Color(0xFF4A5568),
          ),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              '⏰ ${course['start'] ?? ''} - ${course['end'] ?? ''}',
              style: TextStyle(fontFamily: 'Roboto', color: Color(0xFF718096)),
            ),
            Text(
              '📍 ${course['room'] ?? ''}',
              style: TextStyle(fontFamily: 'Roboto', color: Color(0xFF718096)),
            ),
            Text(
              '👨‍🏫 ${course['teacher'] ?? ''}',
              style: TextStyle(fontFamily: 'Roboto', color: Color(0xFF718096)),
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Color(0xFFFDF5E6),
      appBar: AppBar(
        title: Row(
          children: [
            // 由于 primaryColor 未定义，这里使用一个常见的颜色替代，可根据实际需求修改
            // Flutter 的 Colors 类中没有 darkblue 属性，使用 Colors.blue[900] 替代
            Icon(Icons.calendar_today, color: Colors.blue[900]),
            SizedBox(width: 10),
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(widget.username, style: TextStyle(fontSize: 16)),
                Text(
                  // 原代码中 DateFormat 方法是存在的，因为已经导入了 'package:intl/intl.dart'，所以无需修改，直接保留原代码
                  // 由于 DateFormat 已经正确导入，原代码本身无误，可直接保留
                  DateFormat('yyyy年M月d日 EEEE', 'zh_CN').format(DateTime.now()),
                  style: TextStyle(fontSize: 12),
                ),
              ],
            ),
          ],
        ),
        backgroundColor: Colors.white,
        elevation: 0,
        actions: [
          IconButton(
            icon: Icon(Icons.refresh, color: Color(0xFF6C63FF)),
            onPressed: _saveConfigAndFetchSchedule,
          ),
        ],
      ),
      body:
          _isLoading
              ? Center(
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
  runApp(const MaterialApp(home: LoginPage()));
}
