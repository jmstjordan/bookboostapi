
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface ISubscriberService
{
    public Task<Subscriber> UpsertSubscriber(Subscriber subscriber);

    public Task<Subscriber> UpdateSubscriber(string userId, SubscriberPatch subscriber);

    public Task<Subscriber> GetSubscriberByUserId(string userId);

    public Task<Subscriber> GetSubscriberByEmail(string userId);

    public Task AddSubscriber(Subscriber subscriber);
}