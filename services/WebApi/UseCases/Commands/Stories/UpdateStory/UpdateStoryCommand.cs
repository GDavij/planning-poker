namespace WebApi.UseCases.Commands.Stories.UpdateStory;

public record UpdateStoryCommand(string Name, string? StoryNumber, short Order);