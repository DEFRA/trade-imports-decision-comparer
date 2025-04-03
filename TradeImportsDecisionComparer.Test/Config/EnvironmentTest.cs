using Microsoft.AspNetCore.Builder;

namespace TradeImportsDecisionComparer.Test.Config;

public class EnvironmentTest
{

   [Fact]
   public void IsNotDevModeByDefault()
   { 
       var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
       var isDev = TradeImportsDecisionComparer.Config.Environment.IsDevMode(builder);
       Assert.False(isDev);
   }
}
