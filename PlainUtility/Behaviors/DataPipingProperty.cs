using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Behaviors
{
    /// <summary>
    /// <code>
    /// <Canvas>
    ///     <Behaviors:DataPiping.DataPipes>
    ///          <Behaviors:DataPipeCollection>
    ///              <Behaviors:DataPipe Source = "{Binding RelativeSource={RelativeSource AncestorType={x:Type Canvas}}, Path=ActualWidth}"
    ///                          Target="{Binding Path=ViewportWidth, Mode=OneWayToSource}"/>
    ///              <Behaviors:DataPipe Source = "{Binding RelativeSource={RelativeSource AncestorType={x:Type Canvas}}, Path=ActualHeight}"
    ///                          Target="{Binding Path=ViewportHeight, Mode=OneWayToSource}"/>
    ///           </Behaviors:DataPipeCollection>
    ///      </Behaviors:DataPiping.DataPipes>
    /// <Canvas>
    /// </code>
    /// </summary>
    /// <remarks>
    /// <a href="https://stackoverflow.com/questions/1083224/pushing-read-only-gui-properties-back-into-viewmodel">読み取り専用のGUIプロパティをViewModelに戻す</a>
    /// </remarks>
    public class DataPiping
    {
        #region DataPipes (Attached DependencyProperty)

        public static readonly DependencyProperty DataPipesProperty = DependencyProperty.RegisterAttached(
            "DataPipes",
            typeof(DataPipeCollection),
            typeof(DataPiping),
            new UIPropertyMetadata(null));

        public static void SetDataPipes(DependencyObject o, DataPipeCollection value)
        {
            o.SetValue(DataPipesProperty, value);
        }

        public static DataPipeCollection GetDataPipes(DependencyObject o)
        {
            return (DataPipeCollection)o.GetValue(DataPipesProperty);
        }

        #endregion
    }

    public class DataPipeCollection : FreezableCollection<DataPipe>
    {

    }

    public class DataPipe : Freezable
    {
        #region Source (DependencyProperty)

        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source",
            typeof(object),
            typeof(DataPipe),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPipe)d).OnSourceChanged(e);
        }

        protected virtual void OnSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            Target = e.NewValue;
        }

        #endregion

        #region Target (DependencyProperty)

        public object Target
        {
            get { return (object)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            "Target",
            typeof(object),
            typeof(DataPipe), new FrameworkPropertyMetadata(null));

        #endregion

        protected override Freezable CreateInstanceCore()
        {
            return new DataPipe();
        }
    }
}
