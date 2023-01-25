using Caliburn.Micro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Services;
using Yammer.Chat.Core.Test;
using Yammer.Chat.ViewModels;

namespace Yammer.Chat.ViewModels.Test 
{
    [TestClass]
    public class EditProfileViewModelTests : TestBase
    {
        private Mock<INavigator> navigator;
        private Mock<IUserRepository> userRepository;
        private Mock<IProgressIndicator> progressIndicator;
        private Mock<IFileRepository> fileRepository;
        private Mock<IPhotoChooser> photoChooser;

        [TestInitialize]
        public void Init()
        {
            this.navigator = new Mock<INavigator>();
            this.userRepository = new Mock<IUserRepository>();
            this.progressIndicator = new Mock<IProgressIndicator>();
            this.fileRepository = new Mock<IFileRepository>();
            this.photoChooser = new Mock<IPhotoChooser>();
        }

        [TestMethod]
        public async Task saving_saves_changes()
        {
            var originalUser = new User
            {
                Id = 1,
                FirstName = "A",
                LastName = "B",
                JobTitle = "C",
                Summary = "D",
                FullName = "E",
                MobilePhone = "F",
                WorkPhone = "G",
                MugshotTemplate = "H"
            };

            this.userRepository
                .Setup(x => x.UpdateCurrentUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new object()));

            var viewModel = getViewModel();

            viewModel.FirstName = "A2";
            viewModel.LastName = "B2";
            viewModel.JobTitle = "C2";
            viewModel.Summary = "D2";
            viewModel.MobilePhone = "F2";
            viewModel.WorkPhone = "G2";

            await viewModel.Save();

            this.navigator
                .Verify(x => x.GoBack(), Times.Once, "Succesful save should go back");

            this.userRepository
                .Verify(x => x.UpdateCurrentUser("A2", "B2", "C2", "D2", "G2", "F2"),
                Times.Once, "Saving should update current user");
        }

        [TestMethod]
        public async Task unsuccessful_save_does_not_navigate()
        {
            var originalUser = new User();

            this.userRepository
                .Setup(x => x.UpdateCurrentUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws<Exception>();

            var viewModel = getViewModel();

            await viewModel.Save();

            this.navigator
                .Verify(x => x.GoBack(), Times.Never, "Unsuccesful save should not go back");
        }

        [TestMethod]
        public void canceling_does_not_save_changes()
        {
            var originalUser = new User
            {
                Id = 1,
                FirstName = "A",
                LastName = "B",
                JobTitle = "C",
                Summary = "D",
                FullName = "E",
                MobilePhone = "F",
                WorkPhone = "G",
                MugshotTemplate = "H"
            };

            var viewModel = getViewModel();

            viewModel.FirstName = "A2";
            viewModel.LastName = "B2";
            viewModel.JobTitle = "C2";
            viewModel.Summary = "D2";
            viewModel.MobilePhone = "F2";
            viewModel.WorkPhone = "G2";

            viewModel.Cancel();

            this.navigator
                .Verify(x => x.GoBack(), Times.Once, "Canceling should go back");

            this.userRepository
                .Verify(x => x.UpdateCurrentUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never, "Canceling should not update current user");
        }

        private EditProfileViewModel getViewModel()
        {
            return new EditProfileViewModel(this.userRepository.Object, this.navigator.Object, this.progressIndicator.Object, this.fileRepository.Object, this.photoChooser.Object);
        }
    }
}
