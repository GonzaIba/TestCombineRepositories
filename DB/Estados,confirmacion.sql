
  /*
  insert into [salviam2_DBPortalProveedores].[dbo].[EstadoFactura]
  (Nombre)
  values
  ('Creada'),('Confirmada'),('Descargada')

  update [salviam2_DBPortalProveedores].[dbo].[Facturas] set EstadoFacturaFK= case when (Confirmada=1) then 2 else 1 end

    insert into [salviam2_DBPortalProveedores].[dbo].[CentrosDeComputo]
  (Nombre, ApiKey)
  values
  ('Gedeco', '718299460195472e9ad7cd78e6e4d718')

  */