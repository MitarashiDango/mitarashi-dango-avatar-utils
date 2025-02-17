# MitarashiDango's Avatar Utils

## これはなに？

- 自分が欲しいコンポーネントをとりあえず置いておくためのリポジトリー/パッケージ

## なにがあるの？

### Face Emote Control

- アバターの表情コントロールを行う AnimatorController を自動生成するコンポーネント
- 実装されている機能
  - デフォルト表情設定機能
  - ジェスチャーへ表情を割り当てる機能
  - ジェスチャーに割り当てる表情を動的に切り替える機能
  - ジェスチャーでの表情切り替えを一時的にロックする機能
    - アバター頭部を手で触れることでロック状態の切り替えが行えます
    - Sit 判定時にロック状態の切り替えを一時的に無効化するオプションがあります
  - ジェスチャーに割り当てていない追加の表情で固定化する機能

### Animator Controller Modifier

- アバター側の AnimatorController に対して、ビルド時に非破壊で変更を加えるコンポーネント
- 以下の機能を実装済み
  - レイヤーの削除
    - 削除時にダミーレイヤーを挿入するか選択可能
  - 以下の値の上書き
    - デフォルトウェイト
    - アバターマスク
    - ブレンドモード
    - IK Pass

### PhysBones Switcher

- アバターの PhysBone の有効状態を動的に切り替えるメニューおよび AnimatorController を自動生成するコンポーネント

### Export BlendShapes

- 指定したオブジェクトのブレンドシェイプの値をインポート可能な形式でエクスポートする機能
- AnimationClip の生成も可能

### Import BlendShapes

- Export BlendShapes でエクスポートしたブレンドシェイプの値をオブジェクトへ反映する機能

## つかいかた

1. VPM リポジトリー<https://vpm.matcha-soft.com/repos.json>を VCC へ登録する
2. 「MitarashiDango's Avatar Utils」をプロジェクトへ追加する
