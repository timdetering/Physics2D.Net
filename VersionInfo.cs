//
//  The VersionInfo.cs file contains default assembly attributes that describe the common assembly attributes.
//  Assembly specific attributes and values should be defined in the AssemblyInfo.cs file of the individual assemblies.
//  These attributes are placed into the metadata for the assembly that is read by Windows and install programs.
//
//  Based on blog post:
//  http://blog.xoc.net/2012/11/using-assembly-attributes-in-net.html
//
//  To update run:
//  curl -L https://gist.github.com/timdetering/e8cd043606beb2e4d2f8230898e81d37/raw/ > ./VersionInfo.cs
//

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

//
//  The AssemblyCompany attribute contains the company or organization that developed the assembly's legal name.
//  The Application.UserAppDataPath and Application.UserAppDataRegistry properties use the company name in the 
//  directory and registry keys where information is found.
//
[assembly: AssemblyCompany("RD Technologies")]

//
//  AssemblyConfiguration: The AssemblyConfiguration attribute must have the configuration that was used to build the 
//  assembly.
//
//  Use conditional compilation to properly include different assembly configurations.
//
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Retail")]
#endif

//
//  The AssemblyCopyright attribute contains the copyright notice for the assembly.
//  The suggested form is "Copyright © 2020 Acme Corporation", however consult with legal representation for the 
//  exact form that should be used in the country of publication.
//
//  NOTE: A (c) is not a substitution for a © symbol.
//  (See Circular 3: Copyright Notice from the U.S. Copyright Office for information about copyright notices in the 
//  United States, http://www.copyright.gov/circs/circ03.pdf.)
//
[assembly: AssemblyCopyright("Copyright © RD Technologies 2007-2017")]

//
//  The culture that the assembly was built for.
//  The invariant culture that is used by default is indicated by a zero length string.
//  Other cultures are indicated by the RFC 1766 <http://www.ietf.org/rfc/rfc1766.txt> description of the language and 
//  possibly country. Non-invariant cultures are used in satellite assemblies for internationalizing resources.
//
[assembly: AssemblyCulture("")]

//
//  The AssemblyDefaultAlias provides an alternate friendly name for the assembly when the  assembly name file name is 
//  something unfriendly, such as a randomly generated name. Since a randomly generated name would not follow the best 
//  practice for assembly names, it would be highly unusual to need an AssemblyDefaultAlias attribute, and it is 
//  infrequently used.
//
//[assembly: AssemblyDefaultAlias("TODO")]

//
//  A boolean indicating whether to delay sign the assembly.
//  If delay signing is needed, set the value to true; otherwise set it to false. Delayed signing is used when the 
//  author of the assembly does not have access to the private key that will be used to eventually sign the assembly.
//
[assembly: AssemblyDelaySign(false)]

//
//  A short description of the purpose of this assembly.
//  This attribute provides a description of the purpose of this library. This should be a one-sentence or sentence 
//  fragment description, ending in a period.
//
//[assembly: AssemblyDescription("TODO")]

//
//  The product name of the product that this assembly is a part of.
//  The product is the deliverable to the customer.
//
//[assembly: AssemblyProduct("TODO")]

//
//  The friendly name of this particular assembly. A friendly name can contain spaces and other punctuation.
//
//[assembly: AssemblyTitle("TODO")]

//
//  The AssemblyTrademark attribute should be included if any trademarks or service marks are used in the assembly.
//  Trademarks should be stated in complete sentences with a period at the end.
//
[assembly: AssemblyTrademark("BaconBytes is a trademark of RD Technologies.")]

//
//  FxCop CA1016: Mark assemblies with AssemblyVersionAttribute
//  The AssemblyVersion attribute contains a valid version number for the assembly.
//  Applications run only with versions of an assembly for which they were built, by default. It is important to have 
//  the version marked.
//
//  The version number is constructed from four values: 
//  The major and minor version number must be set to the number of the project you are building for. For initial 
//  releases, this should be set to 1.0. Major and minor releases should increment these numbers by one. When the 
//  major version is incremented, the minor version should be reset to zero. For any given release of the product, 
//  these numbers should remain constant.
//
//  The build and revision numbers are numbers that change for each build, alpha, or beta release. These change with 
//  every build of the assembly. The build and release numbers should be specified as an asterisk, which lets the 
//  compiler maintain them.
//
//  The default build number will increment daily. The default revision number is generated from the system clock, so 
//  should be larger on each build. There may be pathological cases where a new build has a lower build and revision 
//  number than a previous build, however, such as two separate builds on different machines where one clock is not 
//  set correctly.
//
//  When the major or minor version number are changed, the AssemblyInformationalVersion attribute should also be 
//  changed.
//
//  Version information for an assembly consists of the following four values:
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
//  You can specify all the values or you can default the Revision and Build Numbers by using the '*' as shown below:
//      [assembly: AssemblyVersion("1.0.*")]
//
[assembly: AssemblyVersion("0.9.*")]

//
//  The AssemblyFileVersion attribute may be set to the version number of the specific file of the assembly.
//  The AssemblyFileVersion attribute is used as the version number of the specific file, as opposed to the assembly. 
//  This version number is used as the Win32 file version in the assembly, which is shown in the Windows interface, as 
//  well as used by some program that read that value. This number cannot contain a wildcard to generate build and 
//  revision numbers, unlike the AssemblyVersion attribute.
//
//  If this attribute is not present, then the AssemblyVersion attribute is also used for the AssemblyFileVersion. 
//  Using the AssemblyVersion for the file version is usually correct, so in most cases this attribute should not be 
//  present.
//
//[assembly: AssemblyFileVersion("1.0.0.0")]

//
//  The AssemblyInformationalVersion attribute should be set to the major and minor version number of the product.
//
//  The Application.UserAppDataPath or Application.UserAppDataRegistry properties reference paths and registry keys by 
//  building a string from the CompanyName, ProductName, and Version number. If this attribute is not present, then 
//  the value in AssemblyVersion attribute is used instead. If the wildcard is used in the AssemblyVersion attribute, 
//  the compiler will change the build and revision number on each build causing information stored on the disk and 
//  registry to be stored in a different directory or registry key.
//
//  The AssemblyInformationalVersion attribute is used to provide a consistent version number for this version of the 
//  product. It should be changed along with the AssemblyVersion attribute number when the major or minor version is 
//  changed.
//
//  When the major or minor version number are changed, the AssemblyInformationalVersion attribute should also be 
//  changed.
//
//  Under some rare circumstances, this might be set to some other value than a pure version number. For example, 
//  suppose that there was a special build for a particular customer that should use separate registry keys or file 
//  paths. In such cases, it can be set to an arbitrary string. However, it will cause a FXCop warning that can be 
//  suppressed. In such a case it might be set like this:
//
//[assembly: AssemblyInformationalVersion("0.9")]
//[assembly: AssemblyInformationalVersion("3.0 CustomerX Build")]

//
//  FxCop CA1014: Mark assemblies with CLSCompliantAttribute
//  Set the assembly to CLS compliant.
//
[assembly: CLSCompliantAttribute(false)]

//
//  FxCop CA1017: Mark assemblies with ComVisibleAttribute
//  The ComVisible attribute must be present and indicates the default visability to COM.
//  Setting ComVisible to false makes the types in this assembly not visible to COM components.
//  If you need to access a type in this assembly from COM, set the ComVisible attribute to true on that type.
//
[assembly: ComVisible(false)]

#if NET_40
//
//  Instructs analysis tools to assume the correctness of an assembly, type, or member without performing static 
//  verification.
//
//  A boolean indicating the default static contract verification.
//  The value passed to the ContractVerification attribute becomes the default state for static analysis of the 
//  assembly. If the value passed is true, then static analysis is performed on the whole assembly, except where 
//  overidden.
//  If the value passed is false, then no static analysis is performed, unless specifically overridden on a given 
//  class or member.
//
//  In most cases the value should be set to true. However if contracts are being added to an existing assembly there 
//  may be hundreds of warnings. In such case, the default might be set to false, and then turned on for a specific 
//  class or method to limit the number of warnings to a manageable level.
//
[assembly: System.Diagnostics.Contracts.ContractVerification(true)]
#endif

//
//  The Guid attribute must be present and assigned to a unique value.
//  The Guid attribute uniquely identifies this assembly to COM. The value must not be copied from any other assembly; 
//  it should be generated by a Guid generator. A Guid generator can be found on the Tool menu in Visual Studio.
//
//[assembly: Guid("TODO")]

//
//  The InternalsVisibleTo attribute may be included to expose internal variables to other libraries.
//  This attribute allows access to the internals of an assembly to another assembly, The most common use of this 
//  attribute is to expose the internals of this assembly to a unit test assembly. If the assembly is properly signed 
//  with a strong name, then the PublicKey property needs to be the public key of the strong name of the assembly 
//  specified.
//
//[assembly: InternalsVisibleTo("TODO.Tests, PublicKey="0023000004800000...309450b2")]

//
//  The NeutralResourcesLanguage attribute is defined in the main assembly and is set to the language of the 
//  resources contained within it.
//  This value is set to the RFC 1766 (http://www.ietf.org/rfc/rfc1766.txt) specifier of the language and country.
//
//  If a main assembly contains resources, the NeutralResourcesLanguageAttribute tells .NET that the neutral resources 
//  are also the resources for that language. This makes it clear to .NET that it does not need to look for a 
//  satellite assembly for that language. In most cases, this will be the codes for the language and country of the 
//  programming team. 
//
//  Even if the assembly doesn't have resources, the attribute indicates the language that the developer's used.
//
[assembly: NeutralResourcesLanguageAttribute("en-US")]

//
//  Set permission to execute but no other permissions.
//  Allows security actions for SecurityPermission to be applied to code using declarative security.
//
//  TODO: Determine the correct attribute
//
//[assembly: System.Security.Permissions.SecurityPermission(
//    System.Security.Permissions.SecurityAction.RequestMinimum, Execution = true)]

//
//  Allows security actions for a PermissionSet to be applied to code using declarative security.
//
//[assembly: PermissionSet(SecurityAction.RequestOptional, Name = "Nothing" )]

/// <summary>
/// Provides access to information about the assembly.
/// </summary>
internal static class AssemblyInfo
{
    /// <summary>
    /// Gets an assembly attribute.
    /// </summary>
    /// <typeparam name="T">
    /// Assembly attribute type.
    /// </typeparam>
    /// <returns>
    /// The assembly attribute of type T.
    /// </returns>
    internal static T Attribute<T>() where T : Attribute
    {
#if NET_45
        return typeof(AssemblyInfo).Assembly.GetCustomAttribute<T>();
#else
        throw new NotImplementedException();
#endif
    }
}