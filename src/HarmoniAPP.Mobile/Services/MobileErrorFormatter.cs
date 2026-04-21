using System.Net;

namespace HarmoniAPP.Mobile.Services;

public static class MobileErrorFormatter
{
    public static string Format(Exception exception) =>
        exception switch
        {
            HttpRequestException { StatusCode: HttpStatusCode.Unauthorized } =>
                "Sua sessão expirou. Entre novamente para continuar.",
            HttpRequestException { StatusCode: HttpStatusCode.Forbidden } =>
                "Seu perfil não tem permissão para acessar este módulo.",
            HttpRequestException { StatusCode: HttpStatusCode.BadRequest } =>
                "Alguns dados enviados não foram aceitos pela API.",
            HttpRequestException =>
                "Não foi possível falar com a API do Harmoni. Verifique se ela está no ar e acessível para o app.",
            InvalidOperationException =>
                exception.Message,
            _ =>
                "Algo saiu do esperado. Tente novamente em instantes."
        };
}
