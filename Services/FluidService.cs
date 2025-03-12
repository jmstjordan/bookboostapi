using BookBoostApi.Interfaces;
using Fluid;


public class FluidService : ITemplateService
{
    private FluidParser _parser = new FluidParser();

    public FluidService()
    {
    }

    public string RenderModel(dynamic model, string inTemplate)
    {
        if (_parser.TryParse(inTemplate, out var template, out var error))
        {   
            var context = new TemplateContext(model);
            return template.Render(context);
        }
        else
        {
            throw new Exception(error);
        }
    }
}
    
