namespace BookBoostApi.Interfaces;

public interface ITemplateService
{
    public string RenderModel(dynamic model, string inTemplate);
}