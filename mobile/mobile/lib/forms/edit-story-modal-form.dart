import 'package:flutter/material.dart';
import 'package:mobile/services/match-service.dart';

import '../models/matches-models.dart';

class EditStoryModal extends StatefulWidget {
  final Map<String, dynamic>? story;
  final Function(Map<String, dynamic>) onSave;

  const EditStoryModal({Key? key, this.story, required this.onSave})
      : super(key: key);

  @override
  State<EditStoryModal> createState() => _EditStoryModalState();
}

class _EditStoryModalState extends State<EditStoryModal> {
  final _formKey = GlobalKey<FormState>();
  final MatchService _matchService = MatchService();
  late TextEditingController _nameController;
  late TextEditingController _storyNumberController;
  bool _isSaving = false;

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(text: widget.story?['name'] ?? '');
    _storyNumberController =
        TextEditingController(text: widget.story?['storyNumber'] ?? '');
  }

  @override
  void dispose() {
    _nameController.dispose();
    _storyNumberController.dispose();
    super.dispose();
  }

  void _saveStory() async {
    if (_formKey.currentState!.validate()) {
      setState(() {
        _isSaving = true;
      });

      final updatedStory = {
        'name': _nameController.text,
        'storyNumber': _storyNumberController.text,
        ...?widget.story,
      };

      try {
        // Call the MatchService to save the story
        if (widget.story == null) {
          await _matchService.createStory(Story.fromJson(updatedStory));
        } else {
          await _matchService.updateStory(Story.fromJson(updatedStory));
        }
        widget.onSave(updatedStory);
        Navigator.pop(context);
      } catch (e) {
        debugPrint('Error saving story: $e');
      } finally {
        setState(() {
          _isSaving = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: MediaQuery.of(context).viewInsets,
      child: SingleChildScrollView(
        child: Container(
          padding: const EdgeInsets.all(16.0),
          child: Form(
            key: _formKey,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  widget.story == null ? 'Create Story' : 'Edit Story',
                  style: const TextStyle(
                      fontSize: 18, fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 16),
                TextFormField(
                  controller: _nameController,
                  decoration: const InputDecoration(labelText: 'Name'),
                  maxLength: 120,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Name of Story is required';
                    }
                    if (value.length > 120) {
                      return 'Name must not exceed 120 characters';
                    }
                    return null;
                  },
                ),
                TextFormField(
                  controller: _storyNumberController,
                  decoration: const InputDecoration(labelText: 'Story Number'),
                  maxLength: 20,
                  validator: (value) {
                    if (value != null && value.length > 20) {
                      return 'Story number must not exceed 20 characters';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 16),
                ElevatedButton(
                  onPressed: _isSaving ? null : _saveStory,
                  child: _isSaving
                      ? const CircularProgressIndicator()
                      : const Text('Save'),
                ),
                const SizedBox(height: 8),
                TextButton(
                  onPressed: () => Navigator.pop(context),
                  child: const Text('Cancel'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}