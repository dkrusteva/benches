#pragma checksum "C:\Users\dkrusteva\source\repos\WalksAndBenches\WalksAndBenches\Views\App\Explore.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f378a979372120db43581d02edcde37e17a66853"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_App_Explore), @"mvc.1.0.view", @"/Views/App/Explore.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/App/Explore.cshtml", typeof(AspNetCore.Views_App_Explore))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\dkrusteva\source\repos\WalksAndBenches\WalksAndBenches\Views\_ViewImports.cshtml"
using WalksAndBenches.Controllers;

#line default
#line hidden
#line 2 "C:\Users\dkrusteva\source\repos\WalksAndBenches\WalksAndBenches\Views\_ViewImports.cshtml"
using WalksAndBenches.Models;

#line default
#line hidden
#line 3 "C:\Users\dkrusteva\source\repos\WalksAndBenches\WalksAndBenches\Views\_ViewImports.cshtml"
using WalksAndBenches.Data.Entities;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f378a979372120db43581d02edcde37e17a66853", @"/Views/App/Explore.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d19a21db86237c82c7fbce7cebdd40385fe00faf", @"/Views/_ViewImports.cshtml")]
    public class Views_App_Explore : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<Uri>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 2 "C:\Users\dkrusteva\source\repos\WalksAndBenches\WalksAndBenches\Views\App\Explore.cshtml"
  
    ViewBag.Title = "Explore walks";

#line default
#line hidden
            BeginContext(70, 19, true);
            WriteLiteral("<div class=\"row\">\r\n");
            EndContext();
#line 6 "C:\Users\dkrusteva\source\repos\WalksAndBenches\WalksAndBenches\Views\App\Explore.cshtml"
     foreach (var p in Model)
    {

#line default
#line hidden
            BeginContext(127, 107, true);
            WriteLiteral("        <div class=\"col-md-5\">\r\n            <div class=\"border bg-light rounded p-1\">\r\n                <img");
            EndContext();
            BeginWriteAttribute("src", " src=\"", 234, "\"", 244, 1);
#line 10 "C:\Users\dkrusteva\source\repos\WalksAndBenches\WalksAndBenches\Views\App\Explore.cshtml"
WriteAttributeValue("", 240, p, 240, 4, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(245, 411, true);
            WriteLiteral(@" class=""img-responsive"" />
                <h3>Royal Fort Gardens</h3>
                <ul>
                    <li>Place: UoB</li>
                    <li>Post Code: BS8 1UB</li>
                    <li>Walk: Circular</li>
                    <li>Sights: university buildings</li>
                </ul>
                <button class=""btn btn-success"">View</button>
            </div>
        </div>
");
            EndContext();
#line 21 "C:\Users\dkrusteva\source\repos\WalksAndBenches\WalksAndBenches\Views\App\Explore.cshtml"
    }

#line default
#line hidden
            BeginContext(663, 8, true);
            WriteLiteral("</div>\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<Uri>> Html { get; private set; }
    }
}
#pragma warning restore 1591
