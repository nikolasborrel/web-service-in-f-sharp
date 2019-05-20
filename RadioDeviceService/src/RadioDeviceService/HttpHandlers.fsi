namespace RadioDeviceService

module HttpHandlers =
    open Microsoft.AspNetCore.Http
    open Giraffe

    val handleGetRadioLocation : 
        ((RadioRepository.RadioID) ->
        HttpFunc -> 
        HttpContext -> 
        System.Threading.Tasks.Task<HttpContext option>)

    val handlePostRadio : ((RadioRepository.RadioID) -> HttpFunc -> HttpContext -> HttpFuncResult)
    val handlePostRadioLocation : ((RadioRepository.RadioID) -> HttpFunc -> HttpContext -> HttpFuncResult)