// Call the dataTables jQuery plugin desc 
$(document).ready(function() {
  $('#dataTable').DataTable({
	  "pageLength": 50,
	  "order": [[ 5, "asc" ]]
  });
});
