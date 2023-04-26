# tecnica_ml

Proyecto realizado en .net 5.0

Proxy encargado de reenviar peticiones GET a la api https://api.mercadolibre.com
Para utilizarlo debemos usar el endpoint Get, que recibe como parametro la url del endpoint al que queremos enviar una request. Este endpoint en nuestro proxy devolverá como response la misma response que provee la api de mercado libre en caso de que el endpoint sea llamado correctamente.
Ejemplo: para llamar al endpoint https://api.mercadolibre.com/sites debemos utilizar el endpoint Get del proxy enviando como parámetro el string "sites"

El endpoint Get devuelve:
- Una response 200 OK en caso de que la petición se reenvíe con éxito a la api de mercado libre
- Una response 400 Bad Request:
    a. Con el mensaje "Hubo un problema: verifique la URL enviada" en caso de que la url enviada sea incorrecta (no exista en la api o tenga parámetros incorrectos)
    b. Con el mensaje "Se superó la cantidad de peticiones por ip de origen" en caso de que se haya superado la cantidad de peticiones permitidas por ip de origen
    c. Con el mensaje "Se superó la cantidad de peticiones al endpoint ___" en caso de que se haya superado la cantidad de peticiones permitidas al endpoint

Este proxy tiene un control de peticiones. Desde el archivo appsettings.json se pueden modificar los valores para estos controles.
MaxPeticionesPorIpOrigen representa el valor máximo de peticiones que pueden hacerse desde una misma ip de origen.
MaxPeticionesPorEndpointDestino reprsenta el valor máximo de peticiones que pueden hacerse a un endpoint en particular.

Tenemos también los endpoints para estadísticas

EstadisticaUsoPorIp recibe por parámetro una ip de origen, y su response nos informa la cantidad de peticiones que se realizaron desde esa ip y el porcentaje que representa sobre el total de peticiones realizadas al proxy.

EstadisticaUsoPorEndpoint recibe por parámetro una url que representa un endpoint (solo el endpoint, sin parámetros), y su response nos informa la cantidad de peticiones que se realizaron a ese endpoint y el porcentaje que representa sobre el total de peticiones realizadas al proxy.

EstadisticaUsoTotal informa el total de peticiones realizadas al proxy.

EstadisticaUsoCorrecto informa la cantidad de peticiones que se realizaron al proxy con éxito, es decir, peticiones que se reenviaron a la api y devolvieron una response, y el porcentaje que representa sobre el total de peticiones realizadas al proxy.

EstadisticaUsoInvalido informa la cantidad de peticiones inválidas que se realizaron al proxy, por enviarse una url con los parámetros incorrectos o apuntar hacia un endpoint que no existe, y el porcentaje que representa sobre el total de peticiones realizadas al proxy.
