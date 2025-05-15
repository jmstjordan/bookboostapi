
namespace BookBoostApi.Interfaces;

public interface ISubscriberService
{
    public Task<Subscriber> UpsertSubscriber(Subscriber subscriber);

    public Task<bool> UpdatePreferences(string userId, Preferences preferences);

    public Task Subscribe(string subscriberId, bool sub);

    public Task<Subscriber> GetSubscriberByUserId(string userId);
}