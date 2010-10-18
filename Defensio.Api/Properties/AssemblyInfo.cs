using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Defensio.Api;

[assembly: AssemblyTitle("Defensio-DotNet")]
[assembly: AssemblyDescription("Defensio-DotNet Client API")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Websense, Inc.")]
[assembly: AssemblyProduct("Defensio")]
[assembly: AssemblyCopyright("Copyright © Websense 2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("6007a2c4-1e77-490e-89d1-90e1bd20523d")]

[assembly: AssemblyVersion(DefensioClient.LibVersion)]

[assembly: WebPermission(SecurityAction.RequestMinimum)]
