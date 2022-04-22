use HealthPetBD

-- Categories
insert into dbo.Categories (description) 
values ('Consulta veterinaria'),('Vacunación en perros y gatos'),('Cirugía veterinaria'),('Castración'),('Hospitalización (emergencia o recuperación de cirugía)'), ('Laboratorio'),('Medicamentos'),('Certificados de viaje')

insert into dbo.PetTypes (description)
values ('Perro'),('Gato'),('Conejo'),('Ave'),('Tortuga'),('Otro')

insert into dbo.Schedules (Schedule)
values ('08:00'), ('09:00'), ('10:00'), ('11:00'), ('13:00'), ('14:00'), ('15:00'), ('16:00')
