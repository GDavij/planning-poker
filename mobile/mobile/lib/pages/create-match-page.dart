import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:mobile/models/matches-models.dart';
import 'package:mobile/services/match-service.dart';

class CreateMatchPage extends StatefulWidget {
  const CreateMatchPage({Key? key}) : super(key: key);

  @override
  State<CreateMatchPage> createState() => _CreateMatchPageState();
}

class _CreateMatchPageState extends State<CreateMatchPage> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController _descriptionController = TextEditingController();
  final MatchService _matchService = MatchService();

  int? _selectedRoleId;
  bool _shouldSpectate = true;
  List<Map<String, dynamic>> _roles = [];

  @override
  void initState() {
    super.initState();
    _fetchRoles();
  }

  Future<void> _fetchRoles() async {
    try {
      final roles = await _matchService.listMatchRoles();
      setState(() {
        _roles = roles
            .map((role) => {
          'roleId': role.roleId,
          'name': role.name,
          'abbreviation': role.abbreviation,
        })
            .toList();
        _selectedRoleId = _roles.isNotEmpty ? _roles[0]['roleId'] : null;
      });
    } catch (e) {
      debugPrint('Error fetching roles: $e');
    }
  }

  void _createMatch() async {
    if (_formKey.currentState!.validate()) {
      try {
        final StartMatchCommandResponse response = await _matchService.startMatchAs(
          _descriptionController.text,
          _selectedRoleId!,
          _shouldSpectate,
        );

        String matchId = response.matchId.toString();
        context.go('/dashboard/matches/join/$matchId');
      } catch (e) {
        debugPrint('Error creating match: $e');
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Create Match')),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              TextFormField(
                controller: _descriptionController,
                decoration: const InputDecoration(
                  labelText: 'Description',
                  border: OutlineInputBorder(),
                ),
                maxLength: 120,
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Description of match is required';
                  }
                  if (value.length > 120) {
                    return 'Description must not exceed 120 characters';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),
              DropdownButtonFormField<int>(
                value: _selectedRoleId,
                decoration: const InputDecoration(
                  labelText: 'Role',
                  border: OutlineInputBorder(),
                ),
                items: _roles.map((role) {
                  return DropdownMenuItem<int>(
                    value: role['roleId'],
                    child: Text(
                      '${role['name']}${role['abbreviation'] != null ? ' - ${role['abbreviation']}' : ''}',
                    ),
                  );
                }).toList(),
                onChanged: (value) {
                  setState(() {
                    _selectedRoleId = value;
                  });
                },
                validator: (value) {
                  if (value == null) {
                    return 'Role is required';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),
              DropdownButtonFormField<bool>(
                value: _shouldSpectate,
                decoration: const InputDecoration(
                  labelText: 'Gonna Observate',
                  border: OutlineInputBorder(),
                ),
                items: const [
                  DropdownMenuItem<bool>(
                    value: false,
                    child: Text('No'),
                  ),
                  DropdownMenuItem<bool>(
                    value: true,
                    child: Text('Yes'),
                  ),
                ],
                onChanged: (value) {
                  setState(() {
                    _shouldSpectate = value!;
                  });
                },
              ),
              const SizedBox(height: 24),
              ElevatedButton(
                onPressed: _createMatch,
                child: const Text('Create'),
                style: ElevatedButton.styleFrom(
                  minimumSize: const Size(double.infinity, 48), // Full width and fixed height
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}