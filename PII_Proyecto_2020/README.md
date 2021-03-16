# Universidad Católica del Uruguay
<img src="https://ucu.edu.uy/sites/all/themes/univer/logo.png"> 

## Facultad de Ingeniería y Tecnologías - Programación II
## Proyecto - bitacoraBot

### Consigna

El chatbot consiste en un bot _"asistente de bitácora"_, especializado en asistir a los alumnos del Taller Interdisciplinario de Introducción a la Ingeniería (TI<sup>3</sup>) con su bitácora personal. #AyudamosALosMasChicos

#### Bitácora

La bitácora es una herramienta de planificación que presenta varias entradas (estilo diario) de distinta índole:

- Objetivos semanales (3 o más, llamados "victorias")

- Planificación semanal día a día

- Reflexión de la planificación semanal (al final de la semana reflexiono sobre la planificación que hice al comienzo)

- Reflexión metacognitiva (luego de cada clase)

El formato actual de la bitácora es un archivo Word con lo último al comienzo que es revisado por los docentes y ayudantes del curso.

#### Asistente de bitácora

El bot asistirá a los participantes del curso en la creación de sus entradas a la bitácora, presentándoles notificaciones luego de las clases para que hagan sus entradas metacognitivas, y los fines de semana para la reflexión y planificación semanal.

Las entradas que se hagan en el chat serán impactadas por el bot en las salidas que correspondan (por ejemplo, en el archivo Word) con el formato correspondiente.

#### Formato

Distintos estudiantes utilizan distintos formatos de bitácora, especialmente para la planificación semanal: algunos usan una tabla, otros listas con puntitos, otros listas en formato bullet journal (círculos para los eventos, puntos para las tareas, asterisco para los ítems importantes), etc.

Además, el bot debe permitir escribir la bitácora en varios formatos distintos; mínimamente en archivos Word y Markdown.

#### Mantenibilidad

El bot debe poder ser capaz de ser modificado mediante configuración para actualizar algunos de sus elementos, por ejemplo, el formato de las entradas (ver _Formato_), si la última entrada va al comienzo o al final, cuándo se envían alertas, el comienzo y final del semestre, entre otros.

Es imprescindible que maneje correctamente las abstracciones necesarias para esto, y que se utilice algún banco de datos externo a la aplicación que permita modificar estos elementos sin cambiar el código fuente.

#### Plataforma

El bot debe poder ser utilizado por consola y desde otras plataformas de chat (por ejemplo, Telegram, WhatsApp, Messenger, etc.). Es necesario integrarlo al menos con una de ellas.


##### Primera entrega
En la primer entrega comenzamos leyendo el problema y empezamos imaginando como podríamos resolverlo.
Comenzamos creando las tarjetas CRC, posteriormente proseguimos a diseñar un diagrama de clases que nos serviría de mucha ayuda posteriormente, el mismo nos genero varíos desafíos ya que al tratar de implementarlo tuvimos en cuenta los principios y patrones que se dieron a lo largo del curso.
Cuando se realizó el primer diagrama de clases se tuvo un panorama mucho más claro de lo que se iba a necesitar posteriormente y que muchas cosas cambiarían, se fue avanzando y retrocediendo, tambien modificando las tarjetas CRC. En estos momentos no hubo grandes inconvenientes ya que no teniamos implementacion, solo nos estabamos preocupando de la especificacion, que es una de las partes mas importantes.
Nos encontramos con varías contradicciones con patrones y principios como Polymorphism, y principios mas complejos como DIP, ISP, tratamos de comenzar simplemente con el patron Expert, y el principio SRP, ya que estabamos seguros que a la hora de avanzar con el proyecto esto iba a cambiar en muchas de las clases, también agregaríamos interfaces, abstracciones y realmente no quisimos adelantarnos.
A lo largo del proyecto los diagramas y la estructura fue cambiando en gran parte, avanzamos sobre la marcha y fuimos resolviendo los desafíos que fueron apareciendo en el camino con el mayor esfuerzo y dedicación.

##### Segunda entrega
Al embarcarnos en la segunda entrega nos encontramos con un panorama mucho más complejo que en la primer entrega, nos enfrentamos con la implementacion, nos fuimos encontrando con dificultades y las fuimos resolviendo, tuvimos que ir modificando muchas de las decisiones que fuimos tomando, agregando, modificando y eliminando muchas clases e interfaces para intentar cumplir con los patrones y principios (SOLID), este fue nuestro objetivo, y sabíamos que si nos enfocabamos en cumplirlo obtendriamos un programa mucho mas eficiente, robusto y sostenible, asi como también sabíamos que no sería para nada facil.
Empezamos a implementar las clases que creamos para la primer entrega, continuamos con la lógica, nos encontramos con dudas que se fueron resolviendo en clase, pero de a poco, al encontrarnos con los desafíos, los fuimos resolviendo de a poco.


##### Conclusiones generales
Consideramos que un buen trabajo para poner en practica todos los conocimientos dados en el correr del curso, debido a que el mismo demandaba todos los conceptos enseñados. En este proyecto se reforzo el trabajo colaborativo, teniendo en cuenta las responsabilidades que cada uno mantenía dentro del proyecto y la parte clave fue que cada uno puso su mayor dedicación para asi poder lograr el objetivo que teníamos en común.
Al final concluimos que fue un proyecto demandante donde pudimos integrar todos los conocimientos, aunque hubieron varios errores durante el proceso del mismo, se pudó estructurar un proyecto que consideramos que cumplió nuestros objetivos.


###### FIT - Universidad Católica del Uruguay
