using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

using Chat.Client.Services.Interfaces;
using Chat.Client.Services;
using Chat.Client.ViewModel;
using Chat.Client.AutoMapperProfiles;

namespace Chat.Client
{
    public partial class App : Application
    {
        private readonly IServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();

            ConfigureServices(services);

            serviceProvider = services.BuildServiceProvider();

            Startup += OnStartup;
        }

        private void ConfigureServices(IServiceCollection services) 
        {
            services.AddSingleton<ChatService>();
            services.AddSingleton<RegistrationChatService>();
            services.AddSingleton<ChatGroupService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IChutConnection, ChutConnection>();

            services.AddTransient<IScrollController, ScrollController>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<RegistarationViewModel>();
            services.AddSingleton<ChatViewModel>();
            services.AddSingleton<UserViewModel>();
            services.AddSingleton<MessageCreaterViewModel>();
            services.AddSingleton<GroupCreaterViewModel>();

            services.AddSingleton<MainWindow>();

            services.AddAutoMapper(typeof(ChatGroupProfile), typeof(ChatMessageProfile), typeof(ChatMemberProfile));
        }

        private void OnStartup(object sernder, StartupEventArgs e) 
        {
            MainWindow window = serviceProvider.GetRequiredService<MainWindow>();

            window.Show();
        }
    }
}
