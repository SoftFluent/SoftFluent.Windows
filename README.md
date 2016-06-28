This repository contains useful controls/converter/classes to work with WPF. The current version of the library is also [available on NuGet](https://www.nuget.org/packages/SoftFluent.Windows/).

# PropertyGrid

This repository contains a customizable `PropertyGrid` for WPF. 

![](https://github.com/SoftFluent/SoftFluent.Windows/wiki/Images/Getting-Started-Result.png)

- Many data types are supported by default: String, Boolean, Date, DateTime, Number, Enum, Multi-Valued Enum, Byte[], Guid, etc.,
- Property names are decamelized by default (FirstName => First Name) unless you use the `DisplayNameAttribute`,
- Common attributes are supported: `DisplayNameAttribute`, `CategoryAttribute`, `BrowsableAttribute`, `ReadOnlyAttribute`,
- Customizable: 
    - `CustomEditor` = `DataTemplate` You can create your own editors to provide a better UX to your user (Slider, Url, Password, etc.) by creating a `DataTemplate`

````xaml
<Grid>
    <Grid.Resources>
        <samples:Customer x:Key="Customer"/>
    </Grid.Resources>
    <windows:PropertyGrid xmlns:windows="clr-namespace:SoftFluent.Windows;assembly=SoftFluent.Windows" 
                            SelectedObject="{StaticResource Customer}" />
</Grid>
````

Go to the [documentation](https://github.com/SoftFluent/SoftFluent.Windows/wiki)

# AutoObject

The `AutoObject` class is a light class which implements `INotifyPropertyChanged` and `IDataErrorInfo` so you can easily and quickly create MVVM like classes with data-binding and validation

# UniversalConverter

Stop writing boring converters, use the `UniversalConverter`!

When you create an application using WPF, you often have to write converters to change one value to the desired type.
The .NET Framework already provides some basics converter such as BooleanToVisibilityConverter.
These converters are very specifics and usually not very configurable.
For example you cannot change the visibility from Collapsed to Hidden.

Let’s see how to use it for a very basic (and to be honest, quite useless) conversion:


````xaml
<Window.Resources>
    <windows:UniversalConverter x:Key="TypeConversionUniversalConverter" />
</Window.Resources>
 
<CheckBox IsChecked="{Binding Converter={StaticResource TypeConversionUniversalConverter},
   Mode=OneWay}" DataContext="true"/>
<CheckBox IsChecked="{Binding Converter={StaticResource TypeConversionUniversalConverter}, 
   Mode=OneWay}" DataContext="yes"/>
````
In this example, UniversalConverter converts the value to the desired type so string values “true” and “yes” will be converted automatically to the Boolean value “true”.

With UniversalConverter, you can create a list of cases, like a C# switch. For instance, this is how we would reproduce the boolean to visibility converter behavior using the UniversalConverter:

````xaml
<windows:UniversalConverter x:Key="BooleanToVisibilityConverter">
    <cfr:UniversalConverter.Switch>
        <cfr:UniversalConverterCase Operator="Equal" Value="True" ConvertedValue="Visible" />
        <cfr:UniversalConverterCase Operator="Equal" Value="False" ConvertedValue="Collapsed" />
    </cfr:UniversalConverter.Switch>
</cfr:UniversalConverter>
````

Like C#, you can use a default value:

````xaml
<cfr:UniversalConverter x:Key="BooleanToVisibilityConverter" DefaultValue="Visible">
    <cfr:UniversalConverter.Switch>
        <cfr:UniversalConverterCase Operator="Equal" Value="False" ConvertedValue="Collapsed" />
    </cfr:UniversalConverter.Switch>
</cfr:UniversalConverter>
````

There are currently these operators available:

* Equal,
* NotEqual,
* GreaterThan,
* GreaterThanOrEqual,
* LesserThan,
* LesserThanOrEqual,
* Between: minimum and maximum value included => [min:max[,
* StartsWith,
* EndsWith,
* Contains,
* IsType: type match exactly,
* IsOfType: type or direved types,
* JavaScript: Yes, you can use JavaScript to evaluate a condition!

With some options:

* StringComparison
* Trim
* Nullify

Here’s a list of examples using different operators.

###Check if a string contains NewLine using JavaScript:

````xaml
<cfr:UniversalConverter x:Key="HasMultipleLinesConverter" DefaultValue="False">
    <cfr:UniversalConverter.Switch>
        <!--look for a CR or LF in the string string-->
        <cfr:UniversalConverterCase Value="/\r|\n/.exec(Value)!=null" ConvertedValue="True" Operator="Javascript" />
    </cfr:UniversalConverter.Switch>
</cfr:UniversalConverter>
````

###Set error message background and foreground color
````xaml
<cfr:UniversalConverter x:Key="ErrorTextBackgroundConverter" DefaultValue="Red">
    <cfr:UniversalConverter.Switch>
        <cfr:UniversalConverterCase Value="" ConvertedValue="Transparent" />
    </cfr:UniversalConverter.Switch>
</cfr:UniversalConverter>
<cfr:UniversalConverter x:Key="ErrorTextForegroundConverter" DefaultValue="White">
    <cfr:UniversalConverter.Switch>
        <cfr:UniversalConverterCase Value="" ConvertedValue="Black" />
    </cfr:UniversalConverter.Switch>
</cfr:UniversalConverter>
````
###Test if a value is over 21
````xaml
<cfr:UniversalConverter x:Key="IsOver21Converter" DefaultValue="false">
    <cfr:UniversalConverter.Switch>
        <cfr:UniversalConverterCase Operator="GreaterThanOrEqual" Value="21" ConvertedValue="true" Options="Convert" />
    </cfr:UniversalConverter.Switch>
</cfr:UniversalConverter>
````
###Is teenager
````xaml
<cfr:UniversalConverter x:Key="IsTeenagerConverter" DefaultValue="false">
    <cfr:UniversalConverter.Switch>
        <cfr:UniversalConverterCase Operator="Between" MinimumValue="13" MaximumValue="20" ConvertedValue="true" Options="Convert" />
    </cfr:UniversalConverter.Switch>
</cfr:UniversalConverter>
````
###Compare types
````xaml
<cfr:UniversalConverter x:Key="TypeConverter" DefaultValue="false">
    <cfr:UniversalConverter.Switch>
        <cfr:UniversalConverterCase Operator="IsType" Value="System.String" ConvertedValue="Type = String" />
        <cfr:UniversalConverterCase Operator="IsType" Value="System.Int32" ConvertedValue="Type = int" />
        <cfr:UniversalConverterCase Operator="IsOfType" Value="BaseClass, MyAssembly" ConvertedValue="Type is of type BaseClass" />
    </cfr:UniversalConverter.Switch>
</cfr:UniversalConverter>
````
###Is empty
````xaml
<cfr:UniversalConverter x:Key="IsEmptyConverter" DefaultValue="Not empty">
    <cfr:UniversalConverter.Switch>
        <cfr:UniversalConverterCase Operator="Equal" Options="Trim, Nullify" ConvertedValue="Empty" />
    </cfr:UniversalConverter.Switch>
</cfr:UniversalConverter>
 
<TextBox x:Name="TextBox"/>
<TextBlock Text="{Binding ElementName=TextBox, Path=Text, Converter={StaticResource IsEmptyConverter}}" />
````
