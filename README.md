APIS [![C#](https://img.shields.io/badge/C%23-7.0-blue)](https://img.shields.io/badge/C%23-7.0-blue)
=================================================================================================================================================================================
**APIS** – Простая и нетребовательная C# библиотека для написания быстрых API.

* [Примеры использования](./tests) (C#)

```C#
public static void Main(string[] args)
{
     var server = new WebServer(IPAddress.Any, 80);
     server.Start();

     server[Method.GET, "/"] = IndexHandler;
}

private static Response IndexHandler(Request request)
{
     return Response.AsHtml("Hello World");
}
```

Получение GET параметров
=================================================================================================================================================================================
GET параметры хранятся в словаре поля **GetParameters** объекта **Request**
```C#
public static void Main(string[] args)
{
     var server = new WebServer(IPAddress.Any, 80);
     server.Start();

     server[Method.GET, "/"] = IndexHandler;
}

private static Response IndexHandler(Request request)
{
     var builder = new StringBuilder();

     foreach (var parameter in request.GetParameters)
     {
          builder.Append($"{parameter.Key} = {parameter.Value}<br>");
     }

     return Response.AsHtml(builder.ToString());
}
```

Получение POST параметров
=================================================================================================================================================================================
POST параметры хранятся в словаре поля **PostParameters** объекта **Request**
```C#
public static void Main(string[] args)
{
     var server = new WebServer(IPAddress.Any, 80);
     server.Start();

     server[Method.POST, "/"] = IndexHandler;
}

private static Response IndexHandler(Request request)
{
     var builder = new StringBuilder();

     foreach (var parameter in request.PostParameters)
     {
          builder.Append($"{parameter.Key} = {parameter.Value}<br>");
     }

     return Response.AsHtml(builder.ToString());
}
```

Шаблонизация ссылок
=================================================================================================================================================================================
Для создания динамических ссылок, в ссылке обработчика параметр шаблона указывается между фигурными скобками.
Параметры шаблонов хранятся в словаре поля **PatternParameters** объекта **Request**

```C#
public static void Main(string[] args)
{
     var server = new WebServer(IPAddress.Any, 80);
     server.Start();

     server[Method.GET, "/{token}/{action}"] = IndexHandler;
}

private static Response IndexHandler(Request request)
{
     var builder = new StringBuilder();

     foreach (var parameter in request.PatternParameters)
     {
          builder.Append($"{parameter.Key} = {parameter.Value}<br>");
     }

     return Response.AsHtml(builder.ToString());
}
```
