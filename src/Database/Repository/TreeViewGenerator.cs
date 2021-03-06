﻿using Database.DicomHelper;
using Database.Objects;
using DIPS.Database.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repository
{
    public class TreeViewGenerator
    {
        private SqlConnection conn = null;

        public ObservableCollection<Patient> GetCollection(SqlDataReader data, Boolean showName, SqlConnection conn)
        {
            this.conn = conn;
            Encryption encryptedName = new Encryption();
            ObservableCollection<Patient> patientCollection = null;

            while (data.Read())
            {
                if (patientCollection == null) patientCollection = new ObservableCollection<Patient>();
                PatientCollection(patientCollection, data, showName);
            }
            data.Close();

            foreach (Patient p in patientCollection)
            {
                foreach (ImageDataset dataSet in p.dataSet) ImageSetCollection(dataSet);
            }

            return patientCollection;
        }

        private void PatientCollection(ObservableCollection<Patient> patientCollection, SqlDataReader data, Boolean showName)
        {
            Encryption encryptedName = new Encryption();
            Patient patient = null;
            ObservableCollection<ImageDataset> dataSetCollection = null;
            Boolean patientExist = false;
            string patientName = data.GetString(data.GetOrdinal("Patient Name"));
            string currentID = data.GetString(data.GetOrdinal("Patient ID"));
            string currentSeries = data.GetString(data.GetOrdinal("Series"));
            int seriesID = data.GetInt32(data.GetOrdinal("Series ID"));

            foreach (Patient p in patientCollection)
            {
                if (p.patientID.Equals(currentID))
                {
                    dataSetCollection = p.dataSet;
                    patientExist = true;
                }
            }

            if (patientExist == false)
            {
                dataSetCollection = new ObservableCollection<ImageDataset>();
                int tableID = data.GetInt32(data.GetOrdinal("Table ID"));
                if (showName == true) patient = new Patient(currentID, encryptedName.Decrypt(patientName, tableID), dataSetCollection);
                else patient = new Patient(currentID, currentID, dataSetCollection);
                patientCollection.Add(patient);
            }

            ImageDataset dataSet = new ImageDataset();
            dataSet.series = currentSeries;
            dataSet.seriesID = seriesID;
            dataSetCollection.Add(dataSet);
        }

        private void ImageSetCollection(ImageDataset dataSet)
        {
            ObservableCollection<PatientImage> imageCollection = new ObservableCollection<PatientImage>();
            dataSet.relatedImages = imageCollection;

            SqlCommand cmd = new SqlCommand("spr_SubTreeView_v001", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@seriesID", SqlDbType.Int).Value = dataSet.seriesID;
            SqlDataReader data = cmd.ExecuteReader();

            while (data.Read())
            {
                PatientImage image = new PatientImage(data.GetInt32(data.GetOrdinal("fileID")));
                imageCollection.Add(image);
            }
            data.Close();
        }
    }
}
