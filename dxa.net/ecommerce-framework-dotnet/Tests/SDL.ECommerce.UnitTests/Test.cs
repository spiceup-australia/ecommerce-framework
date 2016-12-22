﻿namespace SDL.ECommerce.UnitTests
{
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.AutoNSubstitute;

    using SDL.ECommerce.UnitTests.Customizations;
    using SDL.ECommerce.UnitTests.Customizations.Sdl;

    public abstract class Test
    {
        public IFixture Fixture { get; }

        protected Test()
        {
            Fixture = new Fixture().Customize(new AutoNSubstituteCustomization())
                                   .Customize(new MvcCustomization())
                                   .Customize(new SdlCustomizations());
        }
    }

    public abstract class Test<T> : Test
    {
        public T SUT { get; }

        protected Test()
        {
            SUT = Fixture.Create<T>();
        }
    }
}