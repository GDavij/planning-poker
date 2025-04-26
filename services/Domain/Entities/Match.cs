using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;

namespace Domain.Entities;

public class Match : IAggregate
{
    public long MatchId { get; init; }
    public long AccountId { get; init; }
    public Account Account { get; init; }
    public string Description { get; private set; }
    public bool HasStarted { get; private set; }
    public bool HasClosed { get; private set; }

    public ICollection<Participant> Participants { get; init; } = [];
    public ICollection<Story> Stories { get; init; } = [];
    
    private Match()
    { }

    public Match(ICurrentAccount account, string description)
    {
        AccountId = account.AccountId;
        Description = description;
    }

    public void Receive(Participant participant)
    {
        Participants.Add(participant);
    }
    
    public Story? RegisterNewStoryAs(
        string name,
        string? storyNumber,
        INotificationService notificationService)
    {
        if (Stories.Any(s => s.StoryNumber == storyNumber && !string.IsNullOrWhiteSpace(s.StoryNumber)))
        {
            notificationService.AddNotification("Story Number already registered.", "Story.DuplicatedNumber");
        }

        if (notificationService.HasNotifications())
        {
            return null;
        }

        short greatestStoryOrder = 0; 
        if (Stories.Any()) 
        {
            greatestStoryOrder = Stories.Max(s => s.Order);
        }

        var story = new Story(this, name, storyNumber, (byte)(greatestStoryOrder + 1));
        
        Stories.Add(story);

        return story;
    }

    public bool HasAnyStoryInOrderToUpdateThatIsNot(Story storyToUpdate, short newOrder, out Story? storyWithSameOrder)
    {
        var storyWithCondition = Stories.FirstOrDefault(s => s.Order == newOrder &&
                                                               s.StoryId != storyToUpdate.StoryId);

        storyWithSameOrder = storyWithCondition;

        return storyWithCondition is not null;
    }

    public void SwapOrderForStories(Story storyToUpdate, Story storyWithSameOrder)
    {
        short currentStoryToUpdateOrder = storyToUpdate.Order;
        storyToUpdate.MoveTo(storyWithSameOrder.Order);
        storyWithSameOrder.MoveTo(currentStoryToUpdateOrder);
    }

    public Story? GetStoryWithId(long storyId)
    {
        return Stories.FirstOrDefault(s => s.StoryId == storyId);
    }
    
    
    public void RemoveStory(Story story)
    {
        Stories.Remove(story);
        ReorderStories(Stories);
    }
    
    private void ReorderStories(ICollection<Story> stories)
    {
        short order = 1;
        foreach (var story in stories)
        {
            story.MoveTo(order);

            order++;
        }
    }

    public bool HaveAllParticipantsThatAreNotSpectatorsVotedFor(Story story)
    {
        var ableToVoteParticipants = Participants.Where(p => p.IsSpectating is false);

        foreach (var participant in ableToVoteParticipants)
        {
            if (story.HasVoteOf(participant))
            {
                continue;
            }

            return false;
        }

        return true;
    }
}