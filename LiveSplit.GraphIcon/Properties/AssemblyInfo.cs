using LiveSplit.UI.Components;
using System.Reflection;
using System.Runtime.InteropServices;

// Les informations générales relatives à un assembly dépendent de
// l'ensemble d'attributs suivant. Changez les valeurs de ces attributs pour modifier les informations
// associées à un assembly.
[assembly: AssemblyTitle("LiveSplit.GraphIcon")]
[assembly: AssemblyDescription("Component for Livesplit to show a dedicated window with a graph which displays icons of splits")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Hawkrex")]
[assembly: AssemblyProduct("LiveSplit.GraphIcon")]
[assembly: AssemblyCopyright("Copyright ©  2022-2023")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// L'affectation de la valeur false à ComVisible rend les types invisibles dans cet assembly
// aux composants COM. Si vous devez accéder à un type dans cet assembly à partir de
// COM, affectez la valeur true à l'attribut ComVisible sur ce type.
[assembly: ComVisible(false)]

// Le GUID suivant est pour l'ID de la typelib si ce projet est exposé à COM
[assembly: Guid("b6573662-766d-4f96-a81c-542d479bf504")]

// Les informations de version pour un assembly se composent des quatre valeurs suivantes :
//
//      Version principale
//      Version secondaire
//      Numéro de build
//      Révision
//
// Vous pouvez spécifier toutes les valeurs ou indiquer les numéros de build et de révision par défaut
// en utilisant '*', comme indiqué ci-dessous :
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.2")]
[assembly: AssemblyFileVersion("1.0.2")]

[assembly: ComponentFactory(typeof(GraphIconFactory))]