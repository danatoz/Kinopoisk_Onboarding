using Autofac;
using BL.Abstract;
using BL.Concrete;

namespace BL.DependencyResolvers.Autofac;


public class AutofacBusinessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AuthManager>().As<IAuthService>().SingleInstance();

        builder.RegisterType<FilterManager>().As<IFilterService>().SingleInstance();

        builder.RegisterType<MovieManager>().As<IMovieService>().SingleInstance();

        builder.RegisterGeneric(typeof(ApiManager<>)).As(typeof(IApiService<>)).InstancePerLifetimeScope();


        
    }
}