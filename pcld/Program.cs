using System.Text.Json;

namespace PCLD;

class Program
{
    private static readonly HttpClient _httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(30)
    };
    static async Task Main(string[] args)
    {
        Console.WriteLine("text测试123456789abcdefghijklmnopqrstuvwxyz");
        await Task.Delay(1000);
        Console.WriteLine("\r\n " +
            "██████╗  ██████╗██╗     ██████╗ \r\n" +
            "██╔══██╗██╔════╝██║     ██╔══██╗\r\n" +
            "██████╔╝██║     ██║     ██║  ██║\r\n" +
            "██╔═══╝ ██║     ██║     ██║  ██║\r\n" +
            "██║     ╚██████╗███████╗██████╔╝\r\n" +
            "╚═╝      ╚═════╝╚══════╝╚═════╝ \r\n                                ");
        Console.WriteLine("欢迎使用PCLD");
        await Task.Delay(1000);
        Console.WriteLine("免责声明：本程序仅提供Mojang公开api的版本Json文件下载器，完全免费，开源；请勿相信任何第三方下载渠道，程序仅分发于github，由盗版产生的任何问题与作者无关。\r\n\"Minecraft\"以及\"我的世界\"为美国微软公司的商标 开发者与微软公司没有任何关系");
        await Task.Delay(1000);

        while (true)
        {
            Console.Write("请输入要下载的版本号(官方命名规则；输入Q退出)：");
            string Version = Console.ReadLine() ?? "";

            if (Version.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("已退出。");
                break;                    // ← 正常退出,退出码 0
            }

            if (string.IsNullOrWhiteSpace(Version))
            {
                Console.WriteLine("版本号为空,已退出。");
                Console.ReadLine();
                return;
            }

            try
            {
                //获取版本清单
                const string manifestUrl = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
                string manifestJson = await _httpClient.GetStringAsync(manifestUrl);

                using var doc = JsonDocument.Parse(manifestJson);
                var versions = doc.RootElement.GetProperty("versions");

                //找版本
                string? targetUrl = null;
                foreach (var v in versions.EnumerateArray())
                {
                    if (v.GetProperty("id").GetString() == Version)
                    {
                        targetUrl = v.GetProperty("url").GetString();
                        break;
                    }
                }

                if (targetUrl is null)
                {
                    Console.WriteLine($"未找到版本号:{Version}");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine($"已找到 {Version},下载地址:{targetUrl}");

                //下载该 JSON
                string versionJson = await _httpClient.GetStringAsync(targetUrl);

                //保存到本地
                string savePath = Path.Combine(AppContext.BaseDirectory, $"{Version}.json");
                await File.WriteAllTextAsync(savePath, versionJson);

                Console.WriteLine($"下载完成,已保存到:{savePath} \r\n");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"网络错误:{ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON 解析错误:{ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"数据字段缺失:{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误:{ex.Message}");
            }
        }
    }
}