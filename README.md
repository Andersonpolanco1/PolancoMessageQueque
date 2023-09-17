# RabbitMQExample

## Descripcion

Este proyecto es útil para conocer los distintos tipos de exchange en RabbitMQ en C#. En la solución hay un producer, que es una API que simula
una petición, en esa petición hay una acción, la cual se evalúa para enviar a un exchange u otro. El exchange envía el registro a las colas segun la
configuracion en el Binding y el router key. Tambien hay varios consumers (2 por el momento) los cuales estan suscritos a las colas para procesar los mensajes.

Puedes clonar este proyecto y cambiar las configuraciones de los exchange, las colas, router keys y los bindings. Por el momento no se persiste informacion en base de datos, 
quizas mas adelante implemente persistencia con Sql Server, pero eso se escapa de RabbitMQ.

## Requerimientos

* .NET 6
* C#
* RabbitMQ corriendo en el puerto por fececto
