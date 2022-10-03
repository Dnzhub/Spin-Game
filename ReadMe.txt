--------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------

Wheel of fortune

1-Tüm inventory ve item obje dataları scriptable object icerisinde bulunmakta

2-Yeni item eklenmek yada değişiklik yapmak için;
-Scriptable object içerisinde item bilgileri girilip belirtilen item listelerine eklenmelidir(golden-silver-bronz)
-Itemler addressable ve sprite atlas kullandığı için yeni bir item eklerken veya mevcut item bilgilerini değiştirirken
yalnızca item Atlased Sprite Name ismini sprite atlasa kaydedildiği şekilde girmek yeterlidir.

3-Item drop oranları ve miktarları aynı şekilde item scriptable objelerinde bulunmakta

4-Itemler olusturulduktan sonra gerekli item listelerine eklenip WhellOfFortune scripti altında editörde belirtilmelidir.

5-Aynı zamanda addressable sprite atlas objelerinin olusturuldugu atlası tanımlamak icin aynı şekilde editörden atlası belirtip addressable
adresi girilmelidir.

6-Tüm animasyonlar DOTween ile yapıldığı için herhangi bir animator controllera gerek duyulmamaktadır.

--------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------
