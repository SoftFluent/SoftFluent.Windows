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

The `AutoObject` class is a light class which implements `INotifyPropertyChanged` and `IDataErrorInfo` so you can easily and quickly create classes data-binding and validation friendly classes. You can read more on our blog: <http://blog.codefluententities.com/2012/02/08/exploring-the-codefluent-runtime-autoobject>

# UniversalConverter

Stop writing boring converters, use the `UniversalConverter`! You'll find some examples on our blog: <http://blog.codefluententities.com/2015/02/03/wpf-universal-converter>
