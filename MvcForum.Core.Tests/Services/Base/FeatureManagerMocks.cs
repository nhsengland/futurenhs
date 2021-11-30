using Moq;
using MvcForum.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcForum.Core.Tests.Services.Base
{
    internal class FeatureManagerMocks
    {
        internal static Mock<IFeatureManager> GetFeatureManager()
        {
            var folderServiceMocks = new Mock<IFeatureManager>();

            folderServiceMocks.Setup(repo => repo.IsEnabled(It.IsAny<string>())).Returns(
                (string feature) =>
                {
                    return true;
                });

            return folderServiceMocks;
        }
    }
}
