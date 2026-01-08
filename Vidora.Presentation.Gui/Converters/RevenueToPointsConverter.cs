using Microsoft.UI.Xaml.Data; // Thư viện chứa IValueConverter
using Microsoft.UI.Xaml.Media; // Thư viện chứa PointCollection
using Windows.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vidora.Presentation.Gui.Converters
{
    public class RevenueToPointsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is IEnumerable<double> data && data.Any())
            {
                var points = new PointCollection();
                var list = data.ToList();
                double max = list.Max();
                if (max == 0) max = 1; // Tránh chia cho 0

                // Giả sử chiều rộng biểu đồ là 500 và chiều cao là 200 (khớp với XAML của bạn)
                double width = 500;
                double height = 200;
                double stepX = width / (list.Count - 1 == 0 ? 1 : list.Count - 1);

                for (int i = 0; i < list.Count; i++)
                {
                    // Tính tọa độ Y: lấy chiều cao trừ đi giá trị tỉ lệ 
                    // (Vì trong tọa độ máy tính, 0 là ở trên cùng)
                    double y = height - (list[i] / max * height);
                    double x = i * stepX;
                    points.Add(new Point(x, y));
                }
                return points;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
