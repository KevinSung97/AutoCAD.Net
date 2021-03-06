﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MsgBox = System.Windows.Forms.MessageBox;

using Autodesk.AutoCAD.ApplicationServices;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcColor = Autodesk.AutoCAD.Colors;
using AcWindow = Autodesk.AutoCAD.Windows;

namespace AutoCADLibrary
{
    public class PolylineUtil
    {
        /// <summary>
        /// Coordinates의 좌표로 LW폴리선을 만들어줍니다.
        /// </summary>
        /// <param name="Coordinates">Point3d 정점들의 좌표</param>
        /// <param name="LayerName">폴리선에 적용될 레이어 이름</param>
        /// <param name="AcadColor">폴리선에 적용될 색깔</param>
        /// <param name="isClosed">닫힘, 열림 여부</param>
        /// <returns>폴리선을 만들어 리턴하여 줍니다.</returns>
        public static Polyline CreatePolyline(Point3d[] Coordinates, string LayerName, AcColor.Color AcadColor = null, bool isClosed = false)
        {
            try
            {
                if (Coordinates.Length <= 0) throw new System.Exception("정점 갯수가 0입니다.");

                Polyline oPoly = new Polyline();

                for (int i = 0; i < Coordinates.Length; i++) oPoly.AddVertexAt(i, GeometryUtil.ToPoint2d(Coordinates[i]), 0, 0, 0);

                oPoly.Closed = isClosed;
                oPoly.LayerId = LayerUtil.AddLayer(LayerName);
                if (AcadColor != null) oPoly.Color = AcadColor;
                
                return oPoly;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.Print(string.Format("************에러발생************\n위치 : CreatePolyline\n메시지 : {0}", ex.Message));
                return null;
            }
        }

        /// <summary>
        /// Coordinates의 좌표로 LW폴리선을 만들어줍니다.
        /// </summary>
        /// <param name="Coordinates">Point3d 정점들의 좌표</param>
        /// <param name="LayerName">폴리선에 적용될 레이어 이름</param>
        /// <param name="AcadColor">폴리선에 적용될 색깔</param>
        /// <param name="isClosed">닫힘, 열림 여부</param>
        /// <returns>폴리선을 만들어 리턴하여 줍니다.</returns>
        public static Polyline CreatePolyline(Point3dCollection Coordinates, string LayerName, AcColor.Color AcadColor = null, bool isClosed = false)
        {
            try
            {
                if (Coordinates.Count <= 0) throw new System.Exception("정점 갯수가 0입니다.");

                Polyline oPoly = new Polyline();

                for (int i = 0; i < Coordinates.Count; i++) oPoly.AddVertexAt(i, GeometryUtil.ToPoint2d(Coordinates[i]), 0, 0, 0);

                oPoly.Closed = isClosed;
                oPoly.LayerId = LayerUtil.AddLayer(LayerName);
                if (AcadColor != null) oPoly.Color = AcadColor;

                return oPoly;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.Print(string.Format("************에러발생************\n위치 : CreatePolyline\n메시지 : {0}", ex.Message));
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static Polyline ToPolyline(Curve curve)
        {
            if (curve is Polyline3d)
            {
                return Polyline3dToPolyline(curve as Polyline3d);
            }

            if (curve is Polyline2d)
            {
                return Polyline2dToPolyline(curve as Polyline2d);
            }

            return null;
        }

        private static Polyline Polyline3dToPolyline(Polyline3d polyline3d)
        {

                Document doc = DatabaseUtil.GetActiveDocument();
                Database db = doc.Database;
                Polyline oPoly = null;

                try
                {
                    using (doc.LockDocument())
                    {
                        using (Transaction tr = DatabaseUtil.GetTransaction(db))
                        {
                            oPoly = new Polyline();
                            ObjectId[] idVtcies = polyline3d.Cast<ObjectId>().ToArray();

                            for (int i = 0; i < idVtcies.Length; i++)
                            {
                                PolylineVertex3d oVtx = tr.GetObject(idVtcies[i], OpenMode.ForWrite) as PolylineVertex3d;
                                oPoly.AddVertexAt(i, GeometryUtil.ToPoint2d(oVtx.Position), 0, 0, 0);
                            }

                            oPoly.Layer = polyline3d.Layer;
                            oPoly.Color = polyline3d.Color;
                        }
                    }

                    return oPoly;
                }
                catch
                {
                    return null;
                }
            }

        private static Polyline Polyline2dToPolyline(Polyline2d polyline2d)
        {
            Document doc = DatabaseUtil.GetActiveDocument();
            Database db = doc.Database;
            Polyline oPoly = null;

            try
            {
                using (doc.LockDocument())
                {
                    using (Transaction tr = DatabaseUtil.GetTransaction(db))
                    {
                        oPoly = new Polyline();
                        Vertex2d[] oVtcies = polyline2d.Cast<Vertex2d>().ToArray();

                        for (int i = 0; i < oVtcies.Length; i++)
                        {
                            oPoly.AddVertexAt(i, GeometryUtil.ToPoint2d(oVtcies[i].Position), 0, 0, 0);
                        }

                        oPoly.Layer = polyline2d.Layer;
                        oPoly.Color = polyline2d.Color;
                    }
                }

                return oPoly;
            }
            catch
            {
                return null;
            }
        }
    }
}
