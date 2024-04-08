<h1>ChatApp</h1>
<p>ChatApp, Google'ın geliştirdiği GRPC teknolojisini kullanarak sesli ve yazılı iletişimi sağlayan sohbet odaları oluşturmayı amaçlayan bir projedir.</p>

<h2>Amaç</h2>
<p>Bu proje, kullanıcıların sesli ve yazılı iletişim kurabileceği sohbet odaları oluşturmayı hedefler. İstemci ve sunucu arasındaki iletişimde GRPC teknolojisi kullanılır.</p>

<h2>Kullanım</h2>
<p>İstemci uygulamasını açtığınızda, karşılaştığınız ekran kullanıcı girişi için tasarlanmıştır. Burada, uygulamaya giriş yapabilmek için kayıt olmanız veya mevcut bir hesapla giriş yapmanız gerekmektedir.</p>

<p>Ardından, sunucu uygulamasından bir token alırsınız. Bu token, sohbet odalarını oluşturmak veya mevcut sohbet odalarına katılmak için kullanılır.</p>

<p>Herhangi bir sohbet odasına katıldıktan sonra, iki seçenek sunulur: yazılı ve sesli sohbet. Seçtiğiniz seçeneğe göre ilgili streamlere bağlanırsınız.</p>

<h2>Kullanılan Teknolojiler</h2>
<h3>Server</h3>
<ul>
  <li>GRPC</li>
  <li>Protobuf</li>
</ul>
<h3>Client</h3>
<ul>
  <li>Protobuf</li>
  <li>GRPC</li>
  <li>NAudio</li>
</ul>

<h2>Daha Fazla Bilgi</h2>
<p>Bu README dosyası, projenin temel amacını ve kullanımını kapsar. Projeye daha detaylı bir bakış için, lütfen kodları inceleyin.</p>
