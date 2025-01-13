declare module "leaflet.heat" {
  import * as L from "leaflet";

  export function heatLayer(
    latlngs: [number, number, number][],
    options?: {
      minOpacity?: number;
      maxZoom?: number;
      radius?: number;
      blur?: number;
      max?: number;
    }
  ): L.Layer;
}
