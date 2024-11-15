using System.Collections;

namespace XTools.EditorTools
{
    public static class ArrayTools
    {
        public static string ToOneString(this ICollection collection)//扩展方法
        {
            var info = "";
            foreach (var item in collection)
            {
                info += item.ToString() + " ";
            }
            return info;
        }
    }

}
