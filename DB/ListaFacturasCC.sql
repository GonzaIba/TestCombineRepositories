/****** Script for SelectTopNRows command from SSMS  ******/
/****** Listado de facturas para Centro de Computos  ******/
SELECT * FROM 
	[Facturas] as tf
	where
		(tf.CuitDestino='30-71529304-4') 
		and
		(
			(tf.EstadoFacturaFK = 2) --confirmada
			or
			(
				(tf.EstadoFacturaFK=3) --descargada
				and
				(tf.QtyDescargasCC<2)
				and
				not exists (select * from [DescargasFactura] as td where td.FacturaIdFK = tf.Id and td.CentroComputoIdFK = 2)
			)
		)
			

/****** Validacion para descarga de facturas para Centro de Computos  ******/
SELECT * FROM 
	[Facturas] as tf
	where
		(tf.Id = 46) 
		and
		(
			(tf.EstadoFacturaFK = 2) --confirmada
			or
			(
				(tf.EstadoFacturaFK=3) --descargada
				and
				(				
					(tf.QtyDescargasCC<2)
					or
					(
						(tf.QtyDescargasCC>=2)
						and
						exists (select * from [DescargasFactura] as td where td.FacturaIdFK = 46 and td.CentroComputoIdFK = 1)
					)
				)
			)
		)