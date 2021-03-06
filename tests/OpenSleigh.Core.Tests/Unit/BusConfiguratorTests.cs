﻿using System;
using Microsoft.Extensions.DependencyInjection;
using OpenSleigh.Core.DependencyInjection;
using Xunit;

namespace OpenSleigh.Core.Tests.Unit
{
    public class BusConfiguratorTests
    {
        [Fact]
        public void AddSaga_should_fail_if_another_saga_handles_the_same_command()
        {
            var services = new ServiceCollection();

            services.AddOpenSleigh(cfg =>
            {
                cfg.AddSaga<DummySaga, DummySagaState>();

                Assert.Throws<TypeLoadException>(() => cfg.AddSaga<DummySaga, DummySagaState>());
            });
        }
    }
}
