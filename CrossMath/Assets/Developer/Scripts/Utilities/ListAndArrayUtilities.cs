using System.Collections.Generic;

public static class ListAndArrayUtilities {

    public static T RandomOrDefault<T>(this IList<T> list) {
        if (list.Count == 0) {
            return default(T);
        }

        return list[UnityEngine.Random.Range(0, list.Count)];
    }
}