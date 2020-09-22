using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.Appointments;

namespace OpenCurrentZoomMeeting
{
	public static class ZoomOpener
	{
        public static async Task OpenCurrentZoomMeeting()
        {
            var nowTime = DateTime.Now;

            var store = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
            var appointments = (await store.FindAppointmentsAsync(DateTime.Today, TimeSpan.FromDays(1))).ToArray();
            var currentAppointments = appointments.Where(a => a.StartTime <= nowTime && a.StartTime.Add(a.Duration) >= nowTime).ToArray();
            var zoomAppointments = currentAppointments.Select(a => MatchLinkOrNull(a.Location)).Where(link => !string.IsNullOrEmpty(link)).ToArray();

            if(zoomAppointments.Length == 0)
                MessageBox.Show("There are no Zoom meetings currently scheduled");
            else if(zoomAppointments.Length > 1)
                MessageBox.Show("There is more than one Zoom meeting currently scheduled for right now. Please manually select one"); // TODO : UI to select one
            else
                Process.Start(zoomAppointments.Single());
        }

        private static string MatchLinkOrNull(string value)
        {
            var results = Regex.Match(value, "(https{0,1}:\\/\\/iqmetrix.zoom.us\\/\\S*)");
            return results.Success ? results.Value : null;
        }
    }
}
