This repository contains useful controls/converter/classes to work with WPF.

# PropertyGrid

This repository contains a customizable `PropertyGrid` for WPF. 

- Many data types are supported by default: String, Boolean, Date, DateTime, Number, Enum, Multi-Valued Enum, Byte[], Guid, etc.,
- Property names are decamelized by default unless you use the `DisplayNameAttribute`,
- Common attributes are supported: `DisplayNameAttribute`, `DescriptionAttribute`, `CategoryAttribute`, `BrowsableAttribute`, `ReadOnlyAttribute`,
- Customizable: 
    - `CustomEditor` = `DataTemplate` You can create your own editors to provide a better UX to your user (Slider, Url, Password, etc.) by creating a `DataTemplate`,
    - Use `PropertyGridOptionsAttribute` on a property to provide more information to the PropertyGrid

````xaml
<Grid>
    <Grid.Resources>
        <samples:Customer x:Key="Customer"/>
    </Grid.Resources>
    <windows:PropertyGrid xmlns:windows="clr-namespace:SoftFluent.Windows;assembly=SoftFluent.Windows" 
                            SelectedObject="{StaticResource Customer}" />
</Grid>
````

# AutoObject

The `AutoObject` class is a light class which implements INotifyPropertyChanged and IDataErrorInfo so you can easily and quickly create classes data-binding and validation friendly classes. You can read more on our blog: <http://blog.codefluententities.com/2012/02/08/exploring-the-codefluent-runtime-autoobject>

# UniversalConverter

Stop writing boring converters. You'll find some example on our blog: <http://blog.codefluententities.com/2015/02/03/wpf-universal-converter>
