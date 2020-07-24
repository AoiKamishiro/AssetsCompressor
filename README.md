# AssetsCompressor
<H2><a href="https://github.com/AoiKamishiro/UnityCustomEditor_AssetsCompressor/releases">最新版のダウンロードはこちら</a></H2>
<H3>※更新の際は以前のバージョンを削除してからインポートしてください。</H3>
<H3>プログラムについて</H3>
<a>個々にやると面倒な圧縮の設定を、一括して行う事ができます。</a>
<H4>・できること</H4>
<a>アバターやワールドのサイズを抑える事ができます。</a>
<br/>
<a>アップロードやダウンロードにかかる時間を短縮でき、また自分や他人のPCへの描画負荷を少し抑えることができます。</a>
<H4>・やってること</H4>
<a>Unity のプロジェクトに含まれる、画像・音声・3Dモデルのインポート設定を変更しています。</a>
</br>
<a>圧縮を行うので、品質が多少低下します。初期設定では体感で変化のないレベルになっています。</a>
<H3>想定環境</H3>
<a>VRChat用Unity Project</a>
<br/>
<a>Unity2018.4.20f1</a>
<H3>設定の説明</H3>
<H4>・起動方法</H4>
<a>UnityPackageをインポート後、上部メニューの Tools/Kamishiro/AssetsCompressor から起動できます。</a>
<H4>・Texture の設定</H4>
<ul>
<li>既にCrunchCompression済みのテクスチャをスキップ … 手動で圧縮設定を施したファイルを上書きしないようにする設定です。</li>
<li>StreamingMipMaps … VRChatでアバターをアップロードする際に要求されるものです。</li>
<li>CrunchCompression … 画像をほぼ劣化させずにファイルサイズを大きく抑える事ができるようになります。</li>
<li>Maxsize … 画質と引き換えにファイルサイズを小さくします。
</ul>
<h4>・Model の設定</h4>
<ul>
<li>ImportCameras/Lights … モデルに含まれるカメラやライトが読み込まれます。OFF推奨。</li>
<li>MeshCompression … 頂点座標の精度を落としてファイルサイズを押さえます。</li>
<li>Rerad/WriteEnabled … OFFになっていると、VRChatでポリゴン数が取得できず、VeryPoor扱いになった気がします。</li>
<li>OptimizeMash … メッシュの最適化を行います。</li>
<li>GenerateLightmapUVs … VRChatではワールド向けです。アバターではOFF推奨。</li>
</ul>
<h4>・Audio の設定</h4>
<ul>
<li>ForceToMono … 音源をモノラル化します。ファイルサイズが半分くらい減ります。</li>
<li>CompressionFormat … 圧縮方式を変更します。Vorbisが一番抑えられると思います。</li>
<li>Qaulity … 圧縮後の品質を設定します。下げすぎると高音などがカットされます。</li>
<li>SampleRateSetting … サンプリングレートの設定を変更します。元ファイル、自動、手動から選択できます。</li>
</ul>
<H3>連絡先</H3>
<a>Twitter : @aoi3192</a>
<br/>
<a>VRC : 神城 葵[Aoi_JPN]</a>
